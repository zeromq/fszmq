(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Pub-Sub Message Evelopes
====================

Publisher sends multi-part messages where the first frame is the subscription topic
*)
#r "fszmq.dll"
open fszmq
open fszmq.Socket
open System.Threading

let main () =
  // prepare our context and publisher
  use context   = new Context ()
  use publisher = Context.pub context
  Socket.bind publisher "tcp://*:5563"

  while true do
    // write tow messages, each with an envelope and content
    publisher <~| "A"B
              <<| "We don't want to see this."B
    publisher <~| "B"B
              <<| "We would like to see this."B
    Thread.Sleep 1

  0 // return code

(*** hide ***)
main ()
PATH.release ()
