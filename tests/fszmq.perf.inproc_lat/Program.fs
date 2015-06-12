module fszmq.perf.inproc_lat.Program

open fszmq
open fszmq.Message
open System.Diagnostics
open System.Threading

(* _ zeromq ____________________________________________________________ *)

let [<Literal>] ENDPOINT = "inproc://lat_test"

let worker (state:obj) =
  let roundtripCount,context = downcast state
  use socket = Context.rep context
  Socket.connect socket ENDPOINT

  for _ in 1L .. roundtripCount do Message.recv socket ->> socket

let processMessages messageSize roundtripCount socket =
  use message  = new Message(Array.zeroCreate messageSize)
  for i in 1L .. roundtripCount do
    use msgOut = clone message
    msgOut ->> socket
    use msgIn = Message.recv socket
    if size msgIn <> messageSize then failwith "message of incorrect size received"

let runTest messageSize roundtripCount =
  use context = new Context()
  use socket  = Context.req context
  Socket.bind socket ENDPOINT

  let thread = Thread(ParameterizedThreadStart(worker))
  thread.Start((roundtripCount,context))

  printfn "message size: %i [B]" messageSize
  printfn "roundtrip count: %i" roundtripCount

  let watch = Stopwatch.StartNew ()
  processMessages messageSize roundtripCount socket
  watch.Stop()

  let latency = watch.Elapsed.TotalMilliseconds / (float roundtripCount * 2.0)

  thread.Join()

  printfn "average latency: %.3f [us]" latency

(* _ program ___________________________________________________________ *)

let [<Literal>] FAIL = -1
let [<Literal>] OKAY =  0

let (|SizeCount|_|) = function
  | [| size; count; |] -> Some(int size,int64 count)
  | _                  -> None

[<EntryPoint>]
let main = function
  | SizeCount(size,count) ->  runTest size count
                              OKAY
  | _ -> printfn "usage: inproc_lat <message-size> <roundtrip-count>"
         FAIL
