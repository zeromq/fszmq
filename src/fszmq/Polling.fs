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

open System
open System.Collections.Generic

/// for use with the nonce.ZeroMQ.Polling module; associates a callback 
/// with a Socket instance and one or more events, such that the callback 
/// is invoked when the event(s) occurs on the Socket instance
type Poll = Poll of int16 * Socket * (Socket -> unit) with  
  static member Create(events,socket,callback:Action<Socket>) =
    Poll(events,socket,fun s -> callback.Invoke(s))

/// contains methods for working with ZMQ polling capabilities
module Polling =
  
  type private System.Array with
    member self.Limit = self.GetUpperBound(0)

  let private poller (Poll(v,s,_)) = C.zmq_pollitem_t(s.Handle,v)
  let private invoke (Poll(_,s,f)) = f s

  /// performs a single polling run across the give sequence of Poll 
  /// instances, waiting up to the given timeout, and returning
  /// true when one or more callbacks have been invoked, false otherwise  
  [<CompiledName("Poll")>]
  let poll timeout items =
    let items  = items |> Array.ofSeq
    let items' = items |> Array.map poller
    match C.zmq_poll(items',items'.Length,timeout) with
    | 0             ->  false (* pass *)
    | n when n > 0  ->  for i in 0 .. items'.Limit do
                          let e,r = items'.[i].events,items'.[i].revents
                          if e &&& r = e then items.[i] |> invoke
                        true
    | _             ->  raise <| ZeroMQException()
