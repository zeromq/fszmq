(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
open System
open System.IO

let workingFolder =
  match Environment.OSVersion.Platform with
  | PlatformID.Unix 
  | PlatformID.MacOSX ->  "" //NOTE: on WIN we need different paths per architecture
  | _                 ->  if Environment.Is64BitProcess then "x64" else "x86"
  |> sprintf "../../bin/%s"       
//NOTE: fszmq.dll needs to "see" libzmq.dll...
//      force that by running in the deployment staging folder
Environment.CurrentDirectory <- workingFolder 

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
