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

/// <summary>
/// For use with the `Polling` module...
/// <para>Associates a callback with a Socket instance and one or more 
/// events, such that the callback is invoked when the event(s) occurs on 
/// the Socket instance</para>
/// <remarks>NOTE: all sockets passed to `Polling.poll` MUST share the 
/// same context and belong to the thread calling `Polling.poll`</remarks>
/// </summary>
type PollItem = Poll of events * Socket * (Socket -> unit) with  

  static member Create(events,socket,callback:Action<Socket>) =
    Poll(events,socket,fun s -> callback.Invoke(s))

and events = int16

/// Contains methods for working with ZeroMQ's polling capabilities
module Polling =
  
  /// Creates a `PollItem` for the socket which will 
  /// invoke the callback when the socket receives a message.
  let pollIn  fn socket = Poll(ZMQ.POLLIN,socket,fn)
  
  /// Creates a `PollItem` for the socket which will 
  /// invoke the callback when the socket sends a message.
  let pollOut fn socket = Poll(ZMQ.POLLOUT,socket,fn)
  
  /// Creates a `PollItem` for the socket which will 
  /// invoke the callback when the socket sends or receives messages.
  let pollIO  fn socket = Poll(ZMQ.POLLIN ||| ZMQ.POLLOUT,socket,fn)

  type private System.Array with
    member self.Limit = self.GetUpperBound(0)

  let private poller (Poll(v,s,_)) = C.zmq_pollitem_t(s.Handle,v)
  let private invoke (Poll(_,s,f)) = f s

  /// <summary>
  /// Performs a single polling run across the give sequence of 
  /// `PollItem` instances, waiting up to the given timeout. 
  /// <para>Returns `true` when one or more callbacks have been invoked, 
  /// returns `false` otherwise</para>
  /// <remarks>NOTE: All items passed to `Polling.poll` MUST share the same
  /// context and belong to the thread calling `Polling.poll`</remarks>
  /// </summary>
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

  /// Calls `Polling.poll` with the given 
  /// sequence of `PollItem` instances and 0 microseconds timeout.
  [<CompiledName("PollNow")>]
  let pollNow items = poll ZMQ.IMMEDIATE items

  /// Calls `Polling.poll` with the given 
  /// sequence of `PollItem` instances and no timeout, 
  /// effectively causing the polling loop to block indefinitely.
  [<CompiledName("PollForever")>]
  let pollForever items = poll ZMQ.FOREVER items
