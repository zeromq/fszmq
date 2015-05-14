(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
open System

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
