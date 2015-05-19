(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Request-Reply Client
====================

Connects REQ socket to tcp://localhost:5559.

Sends "Hello" to server, expects "World" back.
*)

#r "fszmq.dll"
open fszmq

// helpers to convert between strings and frames
let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString

let main () = 
  use context = new Context ()

  // socket to talk to server
  use requester = Context.req context
  Socket.connect requester "tcp://localhost:5559"

  for request_nbr in 0 .. 9 do
    Socket.send requester (encode "Hello")
    let string = decode (Socket.recv requester)
    printfn "Received reply %d [%s]" request_nbr string

  0 // return code

(*** hide ***)    
main ()
PATH.release ()
