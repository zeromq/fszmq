(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace fszmq.extensions

#nowarn "9" // possible unverifiable IL generation

[<RequireQualifiedAccess>]
module internal C =

  open System
  open System.Runtime.InteropServices

  type HANDLE = nativeint
  
  // NOTE: this will be removed in ZeroMQ v3.x
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_device (int    deviceType, 
                         HANDLE inputScoket, 
                         HANDLE outputSocket);
