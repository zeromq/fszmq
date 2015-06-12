(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Weather Update Proxy
====================

Weather proxy device which does network bridging
*)

#r "fszmq.dll"
open fszmq
open fszmq.Proxying

let main () =
  use context = new Context ()

  // this is where the weather server sits
  use frontend = Context.xsub context
  Socket.connect frontend "tcp://192.168.55.210:5556"

  // this is our public endpoint for subscribers
  use backend = Context.xpub context
  Socket.bind backend "tcp://10.1.1.0:8100"

  // run the proxy until the user interrupts us
  proxy frontend backend None

  0 // return code

(*** hide ***)
main ()
PATH.release ()
