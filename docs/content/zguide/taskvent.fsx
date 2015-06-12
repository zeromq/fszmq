(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Task Ventilator
====================

Binds PUSH socket to tcp://localhost:5557

Sends batch of tasks to workers via that socket
*)
#r "fszmq.dll"
open fszmq
open System
open System.Threading

// helpers to convert between strings and frames
let encode = string >> System.Text.Encoding.ASCII.GetBytes

// initialize random number generator
let rand = Random DateTime.Now.Millisecond

let main () =
  use context = new Context ()

  // Socket to send messages on
  use sender = Context.push context
  Socket.bind sender "tcp://*:5557"

  // Socket to send start of batch message on
  use sink = Context.push context
  Socket.connect sink "tcp://localhost:5558"

  printf "Press Enter when workers are ready: "
  Console.ReadLine () |> ignore
  printfn "Sending tasks to workers..."

  // The first message is "0" and signals start of batch
  Socket.send sink "0"B

  // Send 100 tasks
  let mutable total_msec = 0 // Total expected cost in msecs
  for _ in 0 .. 99 do
    // Random workload from 1 to 100 msecs
    let workload = (rand.Next 100) + 1
    total_msec <- total_msec + workload
    workload |> encode |> Socket.send sender

  printfn "Total expected cost: %d msec" total_msec

  0 // return code

(*** hide ***)
main ()
PATH.release ()
