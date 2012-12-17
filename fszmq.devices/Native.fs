(*-------------------------------------------------------------------------
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace fszmq.devices

open fszmq

open System
open System.Runtime.InteropServices

[<RequireQualifiedAccess>]
module internal C =

  type HANDLE = nativeint
  
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_device (int    deviceType, 
                         HANDLE inputScoket, 
                         HANDLE outputSocket);

/// Functions for working with devices shipped with the native ZMQ library
module PreBuilt =

 (* pre-built devices *)

  let [<Literal>] STREAMER  = 1
  let [<Literal>] FORWARDER = 2
  let [<Literal>] QUEUE     = 3

  /// Starts a built-in ZMQ device running in the current thread 
  /// and returns only if/when the associated context is closed.
  [<CompiledName("Create")>]
  let create deviceType (inputSocket:Socket) (outputSocket:Socket) = 
    C.zmq_device(deviceType,inputSocket.Handle,outputSocket.Handle)

  /// Streamer is a push-pull proxy server
  [<CompiledName("Streamer")>]
  let streamer(front,back) = create STREAMER front back
  
  /// Forwarder is a pub-sub proxy server
  [<CompiledName("Forwarder")>]
  let forwarder(front,back) = create FORWARDER front back
  
  /// Queue is a generic request-reply broker
  [<CompiledName("Queue")>]
  let queue(front,back) = create QUEUE front back
