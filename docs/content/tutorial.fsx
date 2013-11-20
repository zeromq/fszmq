(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

type ENV = System.Environment

ENV.CurrentDirectory <- 
  sprintf @"%s../../bin/zeromq/%s"
          __SOURCE_DIRECTORY__
          (if ENV.Is64BitProcess then "x64" else "x86")

(**
Introducing your project
========================

Say more

*)

#r "fszmq.dll"
open fszmq

printfn "%A" ZMQ.version

(**
Some more info
*)
