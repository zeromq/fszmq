(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

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
