(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Task Sink (design 2)
====================

Binds PULL socket to tcp://localhost:5558

Collects results from workers via that socket

__Update:__

Adds PUB-SUB flow to receive and respond to kill signal
*)
#r "fszmq.dll"
open fszmq
open System.Diagnostics

let main () =
  use context  = new Context ()

  // Socket to receive messages on
  use receiver = Context.pull context
  Socket.bind receiver "tcp://*:5558"

  // Socket for worker control
  use controller = Context.pub context
  Socket.bind controller "tcp://*:5559"

  // Wait for start of batch
  receiver |> Socket.recv |> ignore

  // Start our clock now
  let watch = Stopwatch.StartNew ()

  // Process 100 confirmations
  for task_nbr in 0 .. 99 do
    receiver |> Socket.recv |> ignore
    printf (if (task_nbr / 10) * 10 = task_nbr then ":" else ".")

  // Calculate and report duration of batch
  watch.Stop ()
  printfn "Total elapsed time: %d msec" watch.ElapsedMilliseconds

  // Send kill signal to workers
  Socket.send controller "KILL"B

  0 // return code

(*** hide ***)
main ()
PATH.release ()
