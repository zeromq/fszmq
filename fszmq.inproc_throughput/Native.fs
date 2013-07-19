(*-------------------------------------------------------------------------
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
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
