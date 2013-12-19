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
module fszmq.perf.local_thr.Program

open fszmq
open fszmq.Socket
open fszmq.Timing

(* _ zeromq ____________________________________________________________ *)

let recvMsg messageSize socket =
    let msg = Option.get <| tryRecv socket messageSize ZMQ.WAIT
    //HACK: _technically_ we shouldn't assume tryRecv will always return some. 
    //      But when using the ZMQ.WAIT flag, its a pretty safe bet.
    if Array.length msg <> messageSize then failwith "message of incorrect size received"

let runTest address messageSize messageCount =
  use context = new Context()
  use socket  = Context.pull context
  Socket.bind socket address

  recvMsg messageSize socket
  let elapsed = execTimed (fun () -> for _ in 1L .. (messageCount - 1L) do recvMsg messageSize socket)

  let throughput  = uint32 ((float messageCount) / (float elapsed) * 1000000.0)
  let megabits    = (float (throughput * (uint32 messageSize) * 8u)) / 1000000.0
  
  printfn "message size: %i [B]" messageSize
  printfn "message count: %i" messageCount
  printfn "mean throughput: %d [msg/s]"  throughput
  printfn "mean throughput: %.3f [Mb/s]" megabits 

(* _ program ___________________________________________________________ *)

let [<Literal>] FAIL = -1
let [<Literal>] OKAY =  0

let (|Args|_|) = function
  | [| address; size; count; |] -> Some(address,int size,int64 count)
  | _                           -> None

[<EntryPoint>]
let main = function
  | Args(address,size,count) -> runTest address size count
                                OKAY
  | _ -> printfn "usage: local_lat <bind-address> <message-size> <message-count>"
         FAIL
