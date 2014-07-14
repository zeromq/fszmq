(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
Task Sink
====================

Binds PULL socket to tcp://localhost:5558

Collects results from workers via that socket
*)
#r "fszmq.dll"
open fszmq
open System.Diagnostics

let main () = 
  // Prepare our context and socket
  use context  = new Context ()
  use receiver = Context.pull context
  Socket.bind receiver "tcp://*:5558"

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

  0 // return code

(*** hide ***)
main ()
