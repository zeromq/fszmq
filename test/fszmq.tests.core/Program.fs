(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace fszmq.tests.core

open fszmq
open Utils

module Program = 

  let [<Literal>] OKAY = 0
  let [<Literal>] FAIL = 3

  let scanln = System.Console.ReadLine

  [<EntryPoint>]
  let main _ = 
    let result = ref OKAY

    try
      // pair \ tcp
      "tcp://127.0.0.1:5560" |> basicTests ZMQ.PAIR ZMQ.PAIR
        
      // pair \ inproc
      "inproc://tester" |> basicTests ZMQ.PAIR ZMQ.PAIR

      // req-rep \ tcp
      "tcp://127.0.0.1:5560" |> basicTests ZMQ.REQ ZMQ.REP
        
      // req-rep \ inproc
      "inproc://tester" |> basicTests ZMQ.REQ ZMQ.REP
      
      HighWaterMark.run()

      ShutdownStress.run()

      printfn "SUCCESS! All tests passed."
    
    with
    | x ->  result := FAIL
            printfn "%s" x.Message
            
    printf "press <return> to exit..."
    scanln () |> ignore
    exit !result
