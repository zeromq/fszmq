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
module fszmq.perf.remote_thr.Program

open fszmq
open fszmq.Socket

(* _ zeromq ____________________________________________________________ *)

let runTest address messageSize messageCount =
  use context = new Context()
  use socket = Context.push context
  // Add your socket options here, eg: ZMQ_RATE, ZMQ_RECOVERY_IVL, and ZMQ_MCAST_LOOP (for PGM).
  connect socket address

  let frame = Array.zeroCreate messageSize
  for _ in 1L .. messageCount do frame |>> socket 

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
