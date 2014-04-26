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
module fszmq.perf.inproc.Latency

open fszmq
open System
open System.Diagnostics
open System.Threading

let [<Literal>] LATENCY_PIPE  = "tcp://127.0.0.1:9000"
let [<Literal>] TRIP_COUNT    = 10000L

let MessageSizes = [ 8; 64; 512; 4096; 8192; 16384; 32768; ]

let client () =
  use context = new Context()
  use socket  = Context.req context
  
  Socket.connect socket LATENCY_PIPE

  for msgSize in MessageSizes do
    let msg   = Array.zeroCreate msgSize
    let watch = Stopwatch()

    watch.Start()
    for _ in 0L .. TRIP_COUNT do
      Socket.send socket msg
      Socket.recv socket |> ignore //NOTE: not processing the reply
    watch.Stop()

    let elapsedTime = watch.ElapsedTicks
    Console.WriteLine ("Message size: {0} [B]",msgSize)
    Console.WriteLine ("Roundtrips: {0}",TRIP_COUNT)

    let latency = (float elapsedTime) / (float TRIP_COUNT) / 2.0 * 1000000.0 / (float Stopwatch.Frequency);
    Console.WriteLine ("Your average latency is {0:F2} [us]",latency)

let worker () =
  use context = new Context()
  use socket  = Context.rep context
  
  Socket.bind socket LATENCY_PIPE

  for msgSize in MessageSizes do
    for _ in 0L .. TRIP_COUNT do
      let msg = Socket.recv socket
      Socket.send socket msg

let benchmark = { new ITest with
                    member __.TestName = "Latency Benchmark" 
                    member __.RunTest () =
                      let client = Thread (client,Name="Client")
                      let worker = Thread (worker,Name="Worker")

                      worker.Start ()
                      client.Start ()

                      worker.Join 5000 |> ignore
                      client.Join 5000 |> ignore }
