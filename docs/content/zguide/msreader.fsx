(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
Multi-socket Reader
====================

Reading from multiple sockets. This version uses a simple recv loop.
*)

#r "fszmq.dll"
open fszmq
open System.Threading

let main () = 
  use context = new Context ()

  // connect to task ventilator
  let receiver = Context.pull context
  Socket.connect receiver "tcp://localhost:5557"
  
  // connect to weather server
  let subscriber = Context.sub context
  Socket.connect subscriber "tcp://localhost:5556"
  Socket.subscribe subscriber [ "10001"B ]
  
  let rec getTask () =
    match Socket.tryRecv receiver 255 ZMQ.DONTWAIT with
    | Some msg  ->  (* process task *)
                    getTask ()
    | None      ->  ((* BREAK *))

  let rec getUpdate () =
    match Socket.tryRecv receiver 255 ZMQ.DONTWAIT with
    | Some msg  ->  (* process update *)
                    getUpdate ()
    | None      ->  ((* BREAK *))
  
  // process messages from both sockets
  // we prioritize traffic from the task ventilator
  while true do
    getTask ()
    getUpdate ()  
    // no activity, so sleep for 1 msec
    Thread.Sleep 1

  0 // return code

(*** hide ***)    
main ()
