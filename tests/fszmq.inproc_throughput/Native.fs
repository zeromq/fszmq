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
namespace fszmq.perf.inproc_thr

open fszmq

open System
open System.Runtime.InteropServices

[<RequireQualifiedAccess>]
module internal C =

  type HANDLE = nativeint

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern HANDLE zmq_stopwatch_start ();

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern uint64 zmq_stopwatch_stop (HANDLE watch);

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern void zmq_sleep (int seconds);

//TODO: find a better home for utilities
/// cross-platform timing functions
[<AutoOpen>]
module Utilities =

  // starts the stopwatch; returns the handle to the watch
  [<CompiledName("StartWatch")>]
  let startWatch () = C.zmq_stopwatch_start()

  /// stops the stopwatch; returns the number of microseconds elapsed
  [<CompiledName("StopWatch")>]
  let stopWatch watch = C.zmq_stopwatch_stop(watch)

  /// executes given function, returning elapsed microseconds
  [<CompiledName("ExecuteTimed")>]
  let execTimed action = 
    let watch = startWatch()
    action()
    stopWatch watch
  
  /// sleeps the current thread for given number of seconds
  [<CompiledName("Sleep")>]
  let sleep seconds = C.zmq_sleep(seconds)
