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
open fszmq.Context
open fszmq.Socket
open System.Diagnostics
open System.Threading

(* _ zeromq ____________________________________________________________ *)

let [<Literal>] ENDPOINT = "inproc://thr_test"

let worker (state:obj) =
  let messageSize,messageCount,context = downcast state
  use socket = push context
  connect socket ENDPOINT
  
  for _ in 1L .. messageCount do
    let message = Array.zeroCreate messageSize
    message |>> socket

let checkSize messageSize message =
  if Array.length message <> messageSize 
        then failwith "message of incorrect size received"

let runTest messageSize messageCount =
  let checkSize' = checkSize messageSize

  use context = new Context()
  use socket  = pull context
  bind socket ENDPOINT
  
  let thread = Thread(ParameterizedThreadStart(worker))
  thread.Start((messageSize,messageCount,context))
  
  let message = recv socket
  checkSize' message

  let microsecs = 
    execTimed (fun () ->  for _ in 1L .. (messageCount - 1L) do
                            let message = recv socket
                            checkSize' message)
  thread.Join()

  let throughput = 
    int64 ((float messageCount) / (float microsecs) * 1000000.0)
  let megabits = 
    float ((throughput * (int64 messageSize) * 8L) / 1000000L)

  printfn "mean throughput: %d [msg/s]" throughput
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
