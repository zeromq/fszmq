(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
module fszmq.tests.core.HighWaterMark

open fszmq
open fszmq.Context
open fszmq.Socket
open fszmq.Polling
open System

let run () =

  
  use ctx = new Context(1)
  use s1 = pull ctx
  use s2 = push ctx
  (ZMQ.LINGER,0) |> set s2
  (ZMQ.HWM,5UL)  |> set s2
  "tcp://127.0.0.1:5858" |> bind    s1
  "tcp://127.0.0.1:5858" |> connect s2

  for i in 0 .. 10 do

    let sent = "test"B |> trySend s2 ZMQ.NOBLOCK
     // anything below HWM should be sent
    if i < 5  then assert sent 
              else assert (not sent)
    
  assert // there should be now 5 messages pending, consume one
    match tryRecv s1 ZMQ.BLOCK with
    | Some(_) -> true
    | None    -> false
    
  // now it should be possible to send one more
  let sent = "test"B |> trySend s2 ZMQ.BLOCK
  assert sent
