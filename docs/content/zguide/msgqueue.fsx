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
