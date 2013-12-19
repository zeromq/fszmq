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
module fszmq.perf.remote_lat.Program

open fszmq
open fszmq.Message
open fszmq.Timing

(* _ zeromq ____________________________________________________________ *)

let processMessages messageSize roundtripCount socket =
  let frame = Array.zeroCreate messageSize
  for _ in 1L .. roundtripCount do
    use msgOut = new Message(frame)
    send socket msgOut
    use msgIn = recv socket
    if size msgIn <> messageSize then failwith "message of incorrect size received"

let runTest address messageSize roundtripCount =
  use context = new Context()
  use socket  = Context.req context
  Socket.connect socket address

  let elapsed = execTimed (fun () -> processMessages messageSize roundtripCount socket)
  let latency = float elapsed / (float roundtripCount * 2.0);

  printfn "message size: %d [B]" messageSize
  printfn "roundtrip count: %d" roundtripCount
  printfn "average latency: %.3f [us]" latency
  
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
  | _ -> printfn "usage: remote_lat <connect-address> <message-size> <roundtrip-count>"
         FAIL
