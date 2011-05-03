(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace local_lat

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
    use rep = ctx |> rep
    transport |> bind rep
    
    let mutable msg = Array.empty
    for _ in 0 .. count do
      msg <- rep |> recv
      if msg.Length <> size then
        failwith "message of incorrect size received"
      msg |>> rep
    
    Thread.Sleep(1000) // 1 second pause

  [<EntryPoint>]
  let main = function
    | [| transport; size; count; |] ->
      try
        exec transport (int size) (int count)
        exitPause OKAY
      with 
      | x ->  printfn "%s" x.Message
              exit FAIL
      
    | _ ->  printfn "usage: local_lat <bind-to> <msg-size> <trip-count>"
            exitPause FAIL
