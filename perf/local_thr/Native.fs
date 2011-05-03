(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace local_thr

[<RequireQualifiedAccess>]
module internal C =

  open System.Runtime.InteropServices

  type HANDLE = nativeint
    
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern HANDLE zmq_stopwatch_start()

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern uint32 zmq_stopwatch_stop(HANDLE watch)
