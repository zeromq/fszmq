(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

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
