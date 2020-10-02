(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
A High Available Router Dealer
====================

Connects DEALER socket to tcp://localhost:6666 and tcp://localhost:6667 

Send Hello without loosing message with ZMQ.IMMEDIATE

Cancel and restart r1 and r2 to check the High Availibility behavior

*)

#r @"fszmq.dll"

open fszmq
open fszmq.Context
open fszmq.Socket
open System

let decode x = System.Text.Encoding.UTF8.GetString(x)
let encode x = System.Text.Encoding.UTF8.GetBytes(x:string)

let srecv = recv >> decode

type [<Struct>] Identity = Identity of string
type [<Struct>] Port = Port of int
type [<Struct>] Times = Times of int

type [<Measure>] s
type [<Measure>] ms

type [<Struct>] ThinkTime = ThinkTime of float<s>
let milliseconds = 1000.<ms/s>

let client (ThinkTime thinktime) (Times times) (Identity identity) (Identity dest) ports msg = 
    async {
        use context = new Context ()
        use channel = dealer context
        (ZMQ.IMMEDIATE, 1) |> Socket.setOption channel
        (ZMQ.IDENTITY, identity) |> setOption channel 

        ports |> List.iter (fun (Port port) -> sprintf "tcp://localhost:%i" port |> connect channel)
        do!
            [ 1 .. times ]
            |> List.fold (fun s t -> 
                async {
                    do! s
                    if thinktime > 0.<s> then 
                        //do printfn "%s is sleeping %i" identity t
                        do! milliseconds * thinktime |> int |> Async.Sleep
                    do channel <~| encode dest <<| encode (sprintf "(%i) %s" t msg)
                    printfn "client sent %i" t }) (async.Return ())
    }

let server (Identity identity) ports = 
    async {
        use context = new Context()
        use channel  = dealer context

        (ZMQ.IMMEDIATE, 1) |> Socket.setOption channel
        (ZMQ.IDENTITY, (identity:string)) |> setOption channel 
        
        ports |> List.iter (fun (Port port) -> sprintf "tcp://localhost:%i" port |> connect channel)
        
        let rec handle () = 
            let msg = srecv channel
            printfn "dealer %s received : %s" identity msg
            handle ()
        handle () }

let router (Port port) (token:System.Threading.CancellationToken) = 
    let route channel = 
        let identity = Socket.recv channel |> decode
        let dest = Socket.recv channel
        let client = channel <~| dest
        use message = new Message ()
        Message.recv message channel

        while Message.hasMore message do
            client <~| Message.data message
            |> ignore
            Message.recv message channel
        printfn "routing message from %s and router port %i" identity port
        client <<| Message.data message
    
    async {
        use context = new Context()
        use channel  = router context
        
        sprintf "tcp://*:%i" port |> bind channel
        
        let rec handle () = 
            route channel
            if token.IsCancellationRequested |> not then handle ()
            else printfn "router %i terminated" port
        handle () }

module Async = 
    let start x = 
        let token = new System.Threading.CancellationTokenSource ()
        Async.Start(x token.Token, token.Token)
        token

let pacman = Identity "pacman"
let donkey = Identity "donkey"
let mario = Identity "mario"
let luigi = Identity "luigi"

//Router nodes on Server A and B as Component R
let r1 = Port 6666 |> router |> Async.start
let r2 = Port 6667 |> router |> Async.start

//Services on Server B and C as Component S (connected as R)
server pacman [Port 6666; Port 6667] |> Async.Start
server donkey [Port 6666; Port 6667] |> Async.Start

let send idt dest ports = sprintf "%O hello from %A" DateTime.UtcNow idt |> client (ThinkTime 3.<s>) (Times 20) idt dest ports

//Client on Server D, E, F, G as Component C (connected as R)
send mario pacman [Port 6666; Port 6667] |> Async.Start
send mario donkey [Port 6666; Port 6667] |> Async.Start
send luigi pacman [Port 6666; Port 6667] |> Async.Start
send luigi donkey [Port 6666; Port 6667] |> Async.Start

r1.Cancel()
r2.Cancel()


(*** hide ***)
PATH.release ()
