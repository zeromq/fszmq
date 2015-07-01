module fszmq.perf.local_lat.Program

open fszmq
open fszmq.Message
open System
open System.Threading

(* _ zeromq ____________________________________________________________ *)

let runTest address messageSize roundtripCount =
  use context = new Context()
  use socket  = Context.rep context
  Socket.bind socket address

  use message = new Message ()
  for _ in 1L .. roundtripCount do
    socket |> recv message |> ignore
    if size message <> messageSize then failwith "message of incorrect size received"
    socket <<- message

  Thread.Sleep (TimeSpan.FromMilliseconds 1.0)

(* _ program ___________________________________________________________ *)

let [<Literal>] FAIL = -1
let [<Literal>] OKAY =  0

let (|Args|_|) = function
  | [| address; size; count; |] -> Some(address,int size,int64 count)
  | _                           -> None

[<EntryPoint>]
let main = function
  | Args(address,size,count) -> runTest address size count
                                OKAY
  | _ -> printfn "usage: local_lat <bind-address> <message-size> <roundtrip-count>"
         FAIL
