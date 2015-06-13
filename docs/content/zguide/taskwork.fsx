(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

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
PATH.release ()
