(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Message Queue Broker
====================

Simple message queuing broker, same as request-reply broker but using device.
*)

#r "fszmq.dll"
open fszmq
open fszmq.Proxying

let main () =
  use context = new Context ()

  // socket facing clients
  use frontend = Context.router context
  Socket.bind frontend "tcp://*:5559"

  // socket facing services
  use backend = Context.router context
  Socket.bind backend "tcp://*:5560"

  // start the proxy
  proxy frontend backend None

  0 // return code

(*** hide ***)
main ()
PATH.release ()
