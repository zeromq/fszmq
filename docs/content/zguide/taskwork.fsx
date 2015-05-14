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
Task Worker
====================

Connects PULL socket to tcp://localhost:5557

Collects workloads from ventilator via that socket

Connects PUSH socket to tcp://localhost:5558

Sends results to sink via that socket
*)
#r "fszmq.dll"
open fszmq
open System.Threading

// helpers to convert between strings and frames
let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString

let main () = 
  use context = new Context ()
  
  // Socket to receive messages on
  use receiver = Context.pull context
  Socket.connect receiver "tcp://localhost:5557"

  // Socket to send messages to
  use sender = Context.push context
  Socket.connect sender "tcp://localhost:5558"

  // Process tasks forever
  while true do
    let msg = receiver |> Socket.recv |> decode
    // Simple progress indicator for the viewer
    printf "%s." msg

    // Do the work
    Thread.Sleep (int msg)

    // Send results to sink
    Socket.send sender ""B

  0 // return code
 
(*** hide ***)
main ()
