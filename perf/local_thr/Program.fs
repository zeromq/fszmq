(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace local_thr

open fszmq
open fszmq.Context
open fszmq.Socket
open System
open System.Diagnostics
open System.Threading

module Program =

  let [<Literal>] OKAY = 0
  let [<Literal>] FAIL = 3

  let exitPause code =
    if Debugger.IsAttached then
      printf "\npress <return> to exit..."
      Console.ReadLine() |> ignore
    exit code

  let exec transport size count = 
    
    use ctx = new Context(1)
    use sub = ctx |> sub
    [""B] |> subscribe sub
    transport |> bind sub

    let msg = sub |> recv
    if msg.Length <> size then
      failwith "message of incorrect size received"

    let mutable msg = Array.zeroCreate<byte> size
    let sw = C.zmq_stopwatch_start()
    for _ in 1 .. count do
      msg <- sub |> recv
      if msg.Length <> size then
        failwith "message of incorrect size received"
    let elapsed = C.zmq_stopwatch_stop(sw)
    
    printfn "message size: %d [B]" size
    printfn "message count: %d" count
    
    let throughput = uint32 ((float count) / (float elapsed) * 1000000.0)
    printfn "mean throughput: %d [msg/s]" throughput
    let megabits = float ((throughput * (uint32 size) * 8u) / 1000000u)
    printfn "mean throughput: %.3f [Mb/s]" megabits

  [<EntryPoint>]
  let main = function
    | [| transport; size; count; |] ->
      try
        exec transport (int size) (int count)
        exitPause OKAY
      with 
      | x ->  printfn "%s" x.Message
              exit FAIL
      
    | _ ->  printfn "usage: local_lat <connect-to> <msg-size> <trip-count>"
            exitPause FAIL