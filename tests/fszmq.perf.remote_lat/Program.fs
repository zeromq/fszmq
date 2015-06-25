module fszmq.perf.remote_lat.Program

open fszmq
open fszmq.Message
open System.Diagnostics

(* _ zeromq ____________________________________________________________ *)

let processMessages messageSize roundtripCount socket =
  use message = new Message(Array.zeroCreate messageSize)
  for _ in 1L .. roundtripCount do
    use msg = clone message
    send msg socket 
    socket |> recv msg |> ignore
    if size msg <> messageSize then failwith "message of incorrect size received"

let runTest address messageSize roundtripCount =
  use context = new Context()
  use socket  = Context.req context
  Socket.connect socket address

  let watch = Stopwatch.StartNew ()
  processMessages messageSize roundtripCount socket
  watch.Stop ()

  let latency = watch.Elapsed.TotalMilliseconds / (float roundtripCount * 2.0);

  printfn "message size: %d [B]" messageSize
  printfn "roundtrip count: %d" roundtripCount
  printfn "average latency: %.3f [us]" latency

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
  | _ -> printfn "usage: remote_lat <connect-address> <message-size> <roundtrip-count>"
         FAIL
