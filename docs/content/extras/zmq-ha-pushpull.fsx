(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
A High Available Push Pull using Proxy
====================

Connects PULL socket to tcp://localhost:5572 and tcp://localhost:5574 

Send Hello without loosing message with ZMQ.IMMEDIATE over a PROXY

Cancel and restart p1 and p2 proxies to check the High Availibility behavior

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

let proxy (Port inPort) (Port outPort) token = 
    async {
        use context = new Context ()
        use pullC = Context.pull context
        use pushC = Context.push context
        (ZMQ.IMMEDIATE, 1) |> Socket.setOption pushC
        
        
        inPort |> sprintf "tcp://*:%i" |> Socket.bind pullC 
        outPort |> sprintf "tcp://*:%i" |> Socket.bind pushC

        Async.Start(async {fszmq.Proxying.proxy pushC pullC None}, token)
        do! token.WaitHandle |> Async.AwaitWaitHandle |> Async.Ignore
    }

let receive (Identity identity) ports = 
    use context = new Context ()
    use channel = pull context
    ports |> List.iter (fun (Port port) -> sprintf "tcp://localhost:%i" port |> connect channel)
    
    let rec handle () = 
        let msg = srecv channel
        printfn "%O - I%s: reader receive : %s" (DateTime.Now) identity msg
        handle ()
    handle ()

let send (ThinkTime thinktime) (Times times) ports = 
    async { 
        use context = new Context ()
        use channel = push context
        (ZMQ.IMMEDIATE, 1) |> Socket.setOption channel

        ports |> List.iter (fun (Port port) -> sprintf "tcp://localhost:%i" port |> connect channel)

        let send t = 
            async {
                sprintf "hello %i" t |> encode |> send channel
                printfn "send(%i) finished" t
            }
        do! 
            [1 .. times ] 
            |> List.fold (fun s t -> 
                async { 
                    do! s
                    if thinktime > 0.<s> then do! milliseconds * thinktime |> int |> Async.Sleep
                    do! send t }) (async.Return ()) } 

module Async = 
    let start x = 
        let token = new System.Threading.CancellationTokenSource ()
        Async.Start(x token.Token, token.Token)
        token

//Proxy nodes on server A and B as P component
let p1 = proxy (Port 5571) (Port 5572) |> Async.start
let p2 = proxy (Port 5573) (Port 5574) |> Async.start

//Receiver server C and D as R component (connected to P output)
async { receive (Identity "1") [Port 5572; Port 5574] } |> Async.Start
async { receive (Identity "2") [Port 5572; Port 5574] } |> Async.Start

//Sender server E as S component (connected to P input)
send (ThinkTime 0.5<s>) (Times 200) [Port 5571;Port 5573] |> Async.Start


p1.Cancel()
p2.Cancel()