(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

(**
Node Coordination 
====================

Synchronized publisher
*)
#r "fszmq.dll"
open fszmq

let [<Literal>] SUBSCRIBERS_EXPECTED = 10
// we wait for 10 subscribers

let main () = 
  use context = new Context ()
  
  // socket to talk to clients
  use publisher = Context.pub context
  Socket.setOption publisher (ZMQ.SNDHWM,1100000)
  Socket.bind publisher "tcp://*:5561"

  // socket to receive signals
  use syncservice = Context.rep context
  Socket.bind syncservice "tcp://*:5562"

  // get synchronization from subscribers
  printfn "Waiting for subscribers"
  let rec loop subscribers =
    if subscribers < SUBSCRIBERS_EXPECTED then
      // - wait for synchronization event
      syncservice
      |> Socket.recv
      |> ignore
      // - send synchronization reply
      ""B |> Socket.send syncservice
      loop (subscribers + 1)
  loop 0

  // now broadcast exactly 1M updates followed by END
  printfn "Broadcasting messages"
  for _ in 0 .. 1000000 do 
    "Rhubarb"B |> Socket.send publisher
  "END"B |> Socket.send publisher

  0 // return code

(*** hide ***)
main ()
PATH.release ()
