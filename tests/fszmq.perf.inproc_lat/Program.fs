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
