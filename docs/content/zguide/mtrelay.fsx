(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Multi-threaded relay
====================

Demonstrates inter-thread coordination.
*)
#r "fszmq.dll"
open fszmq
open System.Text

let [<Literal>] STEP2_PIPE = "inproc://step2"
let [<Literal>] STEP3_PIPE = "inproc://step3"

let step1 context = async {
  use xmitter = Context.pair context
  Socket.connect xmitter STEP2_PIPE

  printfn "Step 1 ready, signaling step 2"

  "READY"B |> Socket.send xmitter }

let step2 context = async {
  use receiver = Context.pair context
  Socket.bind receiver STEP2_PIPE

  use xmitter = Context.pair context
  Socket.connect xmitter STEP3_PIPE

  // wait for signal and pass it on
  receiver
  |> Socket.recv
  |> Encoding.ASCII.GetString
  |> ignore

  printfn "Step 2 ready, signaling step 3"

  "READY"B |> Socket.send xmitter }

let main () =
  use context = new Context ()

  Async.Start (step1 context)
  Async.Start (step2 context)

  use receiver = Context.pair context
  Socket.bind receiver STEP3_PIPE

  // wait for signal
  receiver
  |> Socket.recv
  |> ignore

  printfn "Test successful!"

  0 // return code

(*** hide ***)
main ()
PATH.release ()
