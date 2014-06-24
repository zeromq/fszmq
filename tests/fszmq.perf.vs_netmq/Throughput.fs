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
module fszmq.perf.inproc.Throughput

open fszmq
open System
open System.Diagnostics
open System.Threading

let [<Literal>] THROUGHPOUT_PIPE  = "tcp://127.0.0.1:9091"
let [<Literal>] MSG_COUNT         = 1000000L

let MessageSizes = [ 8; 64; 256; 1024; 4096; ]

let rec recvMsg socket msgSize =
  match Socket.tryRecv socket msgSize ZMQ.WAIT with
  | Some msg  -> msg
  | None      -> recvMsg socket msgSize

let ProxyPull () = 
  use context = new Context ()
  use socket  = Context.pull context

  Socket.bind socket THROUGHPOUT_PIPE
  
  for msgSize in MessageSizes do
    let msg = recvMsg socket msgSize
    Debug.Assert (msg.[msgSize / 2] = 0x42uy,"Message did not contain verification data.")

    let watch = Stopwatch ()
    watch.Start ()

    for _ in 1L .. MSG_COUNT do 
      let msg = recvMsg socket msgSize
      Debug.Assert (msg.[msgSize / 2] = 0x42uy,"Message did not contain verification data.")

    watch.Stop ()
    let elapsedTime     = watch.ElapsedTicks
    let msgThroughput   = MSG_COUNT * Stopwatch.Frequency / elapsedTime
    let mbitThroughput  = msgThroughput * (int64 msgSize) * 8L / 1000000L

    Console.WriteLine ("Message size: {0} [B]",msgSize);
    Console.WriteLine ("Average throughput: {0} [msg/s]",msgThroughput)
    Console.WriteLine ("Average throughput: {0} [Mb/s]",mbitThroughput)

let ProxyPush () = 
  use context = new Context ()
  use socket  = Context.push context

  Socket.connect socket THROUGHPOUT_PIPE

  for msgSize in MessageSizes do
    let frm = Array.zeroCreate msgSize
    frm.[msgSize / 2] <- 0x42uy
    for _ in 0L .. MSG_COUNT do 
      use msg = new Message (frm)
      Message.send socket msg

let benchmark = { new ITest with
                    member __.TestName = "Throughput Benchmark" 
                    member __.RunTest () = 
                      let proxyPull = Thread (ProxyPull)
                      let proxyPush = Thread (ProxyPush)
                      
                      proxyPull.Start ()
                      proxyPush.Start ()

                      proxyPull.Join () |> ignore
                      proxyPush.Join () |> ignore }
