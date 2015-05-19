(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Hello World server
====================

Binds REP socket to tcp://*:5555

Expects "Hello" from client, replies with "World"
*)

#r @"fszmq.dll"
open fszmq
open fszmq.Socket
open System.Threading

let main () = 
  // socket to talk to clients
  use context = new Context ()
  use responder = Context.rep context
  Socket.bind responder "tcp://*:5555"

  while true do
    // wait for next request from client
    let _buffer = Socket.recv responder
    printfn "Received Hello"
    
    // do some work
    Thread.Sleep 1000 // msecs

    // send reply back to client
    Socket.send responder "World"B
 
  0 // return code

(*** hide ***)
main ()
PATH.release ()
