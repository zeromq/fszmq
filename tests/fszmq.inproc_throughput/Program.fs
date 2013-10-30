(*-------------------------------------------------------------------------
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
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
