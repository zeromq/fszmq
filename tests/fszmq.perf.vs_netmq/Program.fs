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
module fszmq.perf.inproc.Program
 
open System

let [<Literal>] OKAY = 0

[<EntryPoint>]
let main _ = 
  let tests = [ HelloWorld.benchmark; Latency.benchmark; Throughput.benchmark; ]
  
  for test in tests do
    Console.WriteLine ("Running test {0}...", test.TestName)
    Console.WriteLine ()
    test.RunTest ()
    Console.WriteLine ()

  OKAY  
