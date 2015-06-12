(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Node Coordination
====================

Synchronized subscriber
*)
#r "fszmq.dll"
open fszmq
open System.Threading

let main () =
  use context = new Context ()

  // first, connect our subscriber socket
  use subscriber = Context.sub context
  Socket.connect subscriber "tcp://localhost:5561"
  Socket.subscribe subscriber [| ""B |]

  // 0MQ is so fast, we need to wait a while
  Thread.Sleep 1

  // second, synchronize with publisher
  use syncclient = Context.req context
  Socket.connect syncclient "tcp://localhost:5562"

  // - send a synchronization request
  ""B |> Socket.send syncclient

  // - wait for a synchronization reply
  syncclient
  |> Socket.recv
  |> ignore

  // third, get our updates and report how many we got
  let rec loop update_nbr =
    let msg = Socket.recv subscriber
    match msg with
    | "END"B  ->  update_nbr
    | _       ->  loop (update_nbr + 1)
  printfn "Received %d updates" <| loop 0

  0 // return code

(*** hide ***)
main ()
PATH.release ()
