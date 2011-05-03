(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
module fszmq.tests.core.ShutdownStress

open fszmq
open fszmq.Context
open fszmq.Socket
open fszmq.Polling
open System
open System.Threading

let [<Literal>] THREAD_COUNT  = 10
let [<Literal>] TRANSPORT     = "tcp://127.0.0.1:5560"

let worker (socket:obj) =
  let s2 = socket :?> Socket
  TRANSPORT |> connect s2
  // start closing the socket while the connecting process is underway
  (s2 :> IDisposable).Dispose()
  
let run () =

  for j in 0 .. THREAD_COUNT do

    // check the shutdown with many parallel I/O threads
    let ctx = new Context(7)

    let socket = ctx |> rep
    TRANSPORT |> bind socket

    let spawn _ =
      let s2 = ctx |> req
      let t = Thread(ParameterizedThreadStart(worker))
      t.Start(s2) ;t
    let threads = Array.init THREAD_COUNT spawn

    threads |> Array.iter (fun t -> t.Join())

    (socket :> IDisposable).Dispose()
    (ctx :> IDisposable).Dispose()
