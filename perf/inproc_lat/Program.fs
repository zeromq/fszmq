(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace inproc_lat

open fszmq
open fszmq.Context
open fszmq.Socket
open System
open System.Diagnostics
open System.Threading

module Program =

  let [<Literal>] OKAY = 0
  let [<Literal>] FAIL = 3
  let [<Literal>] TRANSPORT = "inproc://thr_test"

  let exitPause code =
    if Debugger.IsAttached then
      printf "\npress <return> to exit..."
      Console.ReadLine() |> ignore
    exit code

  let exec size count =

    let pump (o:obj) = 
      let ctx = o :?> Context
      use rep = ctx |> rep
      TRANSPORT |> connect rep
      for _ in 0 .. count do
        rep |> recv |> send rep

    use ctx = new Context(1)
    use req = ctx |> req
    TRANSPORT |> bind req

    let t = Thread(ParameterizedThreadStart(pump))
    t.Start(ctx)

    printfn "message size: %d [B]" size
    printfn "message count: %d" count

    let msg1 = Array.zeroCreate size
    let sw = C.zmq_stopwatch_start()
    for _ in 0 .. count do
      msg1 |>> req
      let msg2 = req |> recv
      if msg2.Length <> size then
        failwith  "message of incorrect size received"
    let elapsed = C.zmq_stopwatch_stop(sw)
    t.Join()

    let latency = (float elapsed) / ((float count) * 2.0)
    printfn "average latency: %.3f [us]" latency
    
  [<EntryPoint>]
  let main = function
    | [| size; count;|] ->
      try
        exec (int size) (int count)
        exitPause OKAY
      with 
      | x ->  printfn "%s" x.Message
              exit FAIL

    | _ ->  printfn "usage: inproc_lat <message-size> <roundtrip-count>"
            exitPause FAIL