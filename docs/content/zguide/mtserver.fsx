(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Multi-threaded service
====================

Binds ROUTER socket to tcp://localhost:5555

Passes actual handling of requests to workers on other threads.
*)
#r "fszmq.dll"
open fszmq
open fszmq.Proxying
open System.Text


let [<Literal>] WORKERS_PIPE = "inproc://workers"

let workerRoutine key context = async {
  // socket to talk to dispatcher
  use receiver = Context.rep context
  Socket.connect receiver WORKERS_PIPE

  while true do
    receiver
    |> Socket.recv
    |> Encoding.ASCII.GetString
    |> printfn "(%i) Received request: [%s]" key
    // do some 'work'
    do! Async.Sleep 1
    // sned reply back to client
    Socket.send receiver "World"B
  }

let main () =
  use context = new Context ()

  // socket to talk to clients
  use clients = Context.router context
  Socket.bind clients "tcp://*:5555"

  // socket to talk to workers
  use workers = Context.dealer context
  Socket.bind workers WORKERS_PIPE

  // launch pool of worker threads
  for key in 0 .. 5 do
    Async.Start (workerRoutine key context)

  // connect worker threads to client requests via a queue proxy
  proxy clients workers None

  0 // return code

(*** hide ***)
main ()
PATH.release ()
