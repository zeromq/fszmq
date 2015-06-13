(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

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
PATH.release ()
