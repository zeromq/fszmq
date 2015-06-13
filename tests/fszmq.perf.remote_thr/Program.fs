module fszmq.perf.remote_thr.Program

open fszmq
open fszmq.Socket

(* _ zeromq ____________________________________________________________ *)

let runTest address messageSize messageCount =
  use context = new Context()
  use socket = Context.push context
  // Add your socket options here, eg: ZMQ_RATE, ZMQ_RECOVERY_IVL, and ZMQ_MCAST_LOOP (for PGM).
  connect socket address

  let frame = Array.zeroCreate messageSize
  for _ in 1L .. messageCount do frame |>> socket

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
  | _ -> printfn "usage: local_lat <bind-address> <message-size> <message-count>"
         FAIL
