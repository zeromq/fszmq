(* ------------------------------------------------------------------------
This file is part of fszmq.

fszmq is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published 
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

fszmq is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with fszmq. If not, see <http://www.gnu.org/licenses/>.

Copyright (c) 2011-2013 Paulmichael Blasucci
------------------------------------------------------------------------ *)
module fszmq.perf.inproc_thr.Program

open fszmq
open fszmq.Socket
open fszmq.Timing
open System.Diagnostics
open System.Threading

(* _ zeromq ____________________________________________________________ *)

let [<Literal>] ENDPOINT = "inproc://thr_test"

let worker (state:obj) =
  let messageSize,messageCount,context = downcast state
  use socket = Context.push context
  connect socket ENDPOINT
  let frame = Array.zeroCreate messageSize
  for _ in 1L .. messageCount do frame |>> socket 

let recvMsg messageSize socket =
    let msg = Option.get <| tryRecv socket messageSize ZMQ.WAIT
    //HACK: _technically_ we shouldn't assume tryRecv will always return some. 
    //      But when using the ZMQ.DONTWAIT flag, its a pretty safe bet.
    if Array.length msg <> messageSize then failwith "message of incorrect size received"

let runTest messageSize messageCount =
  use context = new Context()
  use socket  = Context.pull context
  bind socket ENDPOINT
  
  printfn "message size: %i [B]"  messageSize
  printfn "message count: %i"     messageCount

  let thread = Thread(ParameterizedThreadStart(worker))
  thread.Start((messageSize,messageCount,context))
  
  recvMsg messageSize socket
  let elapsed = execTimed (fun () -> for _ in 1L .. (messageCount - 1L) do recvMsg messageSize socket)

  thread.Join()

  let throughput  = uint32 ((float messageCount) / (float elapsed) * 1000000.0)
  let megabits    = (float (throughput * (uint32 messageSize) * 8u)) / 1000000.0

  printfn "mean throughput: %d [msg/s]"  throughput
  printfn "mean throughput: %.3f [Mb/s]" megabits 

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
  | _ -> printfn "usage: inproc_thr <message-size> <message-count>"
         FAIL
