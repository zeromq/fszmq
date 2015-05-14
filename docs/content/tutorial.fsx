(*** hide ***)
// do some environmental setup
#I "../../bin"
open System

let workingFolder =
  match Environment.OSVersion.Platform with
  | PlatformID.Unix 
  | PlatformID.MacOSX ->  "" //NOTE: on WIN we need different paths per architecture
  | _                 ->  if Environment.Is64BitProcess then "x64" else "x86"
  |> sprintf "../../bin/%s"
//NOTE: fszmq.dll needs to "see" libzmq.dll...
//      force that by running in the deployment staging folder
Environment.CurrentDirectory <- workingFolder 

let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString 

open System.Threading
let sleepms ms  = Thread.Sleep(int ms)
let spawn  fn   = Thread(ThreadStart fn).Start()
let spawnp fn o = Thread(ParameterizedThreadStart fn).Start(o)

(**
"Hello, World" with fszmq
========================

As a gentle introduction to using fszmq, we'll create a simple client\server example.
Specifically, we'll create two sockets. One socket, acting as the server, will _bind_ to a TCP endpoint.
Whenever it gets a request message it will take some action. Meanwhile, the other socket will act as a client.
It will _connect_ to the server's TCP endpoint and issue a number of requests. For each reply, it will take some action.

To start, we'll reference our library and open the `fszmq` namespace, which defines the `Socket` type and the `Context` type.
_(Note: don't worry too much about contexts just yet. For now, assume every node in your network has one `Context` instance
which owns one, or more, `Socket` instances.)_ We'll also open some modules which contain functions for working with contexts and sockets.

*)
#r "fszmq.dll"
open fszmq
open fszmq.Context
open fszmq.Socket

(**
Now here's our basic server that takes "hello" requests and replies with "world". Any other message causes it to exit.
*)

let server () =
  // create a ZMQ context
  use context = new Context()
  
  // create reply socket
  use server  = rep context
  // begin receiving connections
  bind server "tcp://*:5555"
  
  let rec loop () =
    // process request (i.e. 'recv' a message from our 'server')
    // NOTE: it's convenient to 'decode' the (binary) message into a string
    match server |> recv |> decode with
    | "hello"   ->  // valid request; send a reply back
                    // NOTE: "..."B is short-hand for a byte array of ASCII-encoded chars
                    "world"B |>> server
                    // wait for next request
                    loop() 
    | _         ->  // invalid request; stop receiving connections
                    "goodbye"B |>> server 

  // wait for next request
  loop () 

(**
And here's a simple client that makes 10 requests and prints the server's reply each time.
*)

let client () =
  // create a ZMQ context
  use context = new Context()
  
  // create a request socket
  use client  = req context
  // connect to the server
  "tcp://localhost:5555" |> connect client

  for i in 1 .. 10 do
    // 'send' a request to the server
    let request = if i = 10 then "goodbye" else "hello"
    // NOTE: we need to 'encode' a string to binary (before transmission)
    request |> encode |> send client
    printfn "(%i) sent: %s" i request
    // receive and print a reply from the server
    let reply = (recv >> decode) client
    printfn "(%i) got: %s" i reply

(*** hide, define-output: client ***)
spawn server
client()

(**
If you run this example in F# Interactive, you should see the following:
*)
(*** include-output: client ***)

(**
Notice how our two sockets are communicating synchronously. In other words, the client sends one request and must wait for a reply.
Conversely, the server waits for a single request and immediately responds to it. If this were not the case, the parenthetical numbers
at the start of each output line could be out of order. 

But don't think this is the only way to use fszmq! Go look at the other [samples][content], or read the [zguide][zguide], 
to see examples of asynchronous client\server, publish\subscribe, map\reduce, and many other distributed computing patterns. 
And, most of all, have fun using F# ("Simple code for complex problems") and ZeroMQ ("Distributed computing made simple")!

  [content]: https://github.com/zeromq/fszmq/tree/master/docs/content
  [zguide]: http://zguide.zeromq.org/page:all
*)
