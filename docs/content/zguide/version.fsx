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
0MQ Version
====================

Displays the version of 0MQ currently being used
*)

#r "fszmq.dll"
open fszmq

let main () = 
  match ZMQ.version with
  | Version (m,n,p) -> printfn "Current 0MQ version is %d.%d.%d" m n p
  | Unknown         -> printfn "Unable to determine current 0MQ version"
  
  0 // return code
  
(*** hide ***) 
main ()
