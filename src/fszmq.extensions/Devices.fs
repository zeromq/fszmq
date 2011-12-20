(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace fszmq

module Devices =

 (* pre-built devices *)

  // NOTE: all device-related functionality will be removed in ZeroMQ v3.x 
  let [<Literal>] STREAMER  = 1
  let [<Literal>] FORWARDER = 2
  let [<Literal>] QUEUE     = 3

  /// Starts a built-in ZMQ device running in the current thread 
  /// and returns only if/when the associated context is closed.
  [<CompiledName("Create")>]
  let create deviceType (inputSocket:Socket) (outputSocket:Socket) = 
    // HACK: access to libzmq.dll (P/Invoke) will be removed in 3.x branch
    fszmq.extensions.C.zmq_device(deviceType,
                                  inputSocket.Handle,
                                  outputSocket.Handle)

  /// Streamer is a push-pull proxy server
  [<CompiledName("Streamer")>]
  let streamer(front,back) = create STREAMER front back
  
  /// Forwarder is a pub-sub proxy server
  [<CompiledName("Forwarder")>]
  let forwarder(front,back) = create FORWARDER front back
  
  /// Queue is a generic request-reply broker
  [<CompiledName("Queue")>]
  let queue(front,back) = create QUEUE front back
