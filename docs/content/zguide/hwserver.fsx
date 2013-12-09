(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
Hello World server
====================

Binds REP socket to tcp://*:5555

Expects "Hello" from client, replies with "World"
*)

#r @"fszmq.dll"
open fszmq
open fszmq.Socket
open fszmq.Timing

let main () = 
  // socket to talk to clients
  use context = new Context()
  use responder = Context.rep context
  Socket.bind responder "tcp://*:5555"

  while true do
    // wait for next request from client
    let _buffer = Socket.recv responder
    printfn "Received Hello"
    
    sleep 1 // do some work

    // send reply back to client
    Socket.send responder "World"B
 
  0 // return code

(*** hide ***)
main ()
