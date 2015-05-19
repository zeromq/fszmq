(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Request-Reply Worker
====================

Connects REP socket to tcp://*:5560.

Expects "Hello" from client, replies with "World".
*)

#r "fszmq.dll"
open fszmq
open System.Threading

// helpers to convert between strings and frames
let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString

let main () = 
  use context = new Context ()
  
  // socket to talk clients
  use responder = Context.rep context
  Socket.connect responder "tcp://localhost:5560"

  while true do
    // wait for next request from client
    let string = decode (Socket.recv responder)
    printfn "Received request: [%s]" string

    // do some 'work'
    Thread.Sleep 1

    // send reply back to client
    Socket.send responder (encode "World")
    
  0 // return code

(*** hide ***)    
main ()
PATH.release ()
