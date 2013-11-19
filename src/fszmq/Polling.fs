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
namespace fszmq

open System
open System.Collections.Generic
open System.Runtime.CompilerServices

/// <summary>
/// For use with the Poll module...
/// <para>Associates a callback with a Socket instance and one or more 
/// events, such that the callback is invoked when the event(s) occurs on 
/// the Socket instance</para>
/// <remarks>NOTE: all sockets passed to Polling.poll MUST share the 
/// same context and belong to the thread calling Polling.poll</remarks>
/// </summary>
type Poll = Poll of events * Socket * (Socket -> unit) with  

  /// Creates a poll item in a way friendly to languages other then F#
  static member Create(events,socket,callback:Action<Socket>) =
    Poll(events,socket,fun s -> callback.Invoke(s))

and events = int16

/// Contains methods for working with ZMQ's polling capabilities
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Polling =
  
  /// Creates a Poll item for the socket which will 
  /// invoke the callback when the socket receives a message
  [<CompiledName("PollIn")>]
  let pollIn fn socket = Poll(ZMQ.POLLIN,socket,fn)
  
  /// Creates a Poll item for the socket which will 
  /// invoke the callback when the socket sends a message
  [<CompiledName("PollOut")>]
  let pollOut fn socket = Poll(ZMQ.POLLOUT,socket,fn)
  
  /// Creates a Poll item for the socket which will 
  /// invoke the callback when the socket sends or receives messages
  [<CompiledName("PollInOut")>]
  let pollIO fn socket = Poll(ZMQ.POLLIN ||| ZMQ.POLLOUT,socket,fn)

  let private poller (Poll(v,s,_)) = C.zmq_pollitem_t(s.Handle,v)
  let private invoke (Poll(_,s,f)) = f s

  /// <summary>
  /// Performs a single polling run across the give sequence of 
  /// Poll items, waiting up to the given timeout. 
  /// <para>Returns true when one or more callbacks have been invoked, 
  /// returns false otherwise</para>
  /// <remarks>NOTE: All items passed to Polling.poll MUST share the same
  /// context and belong to the thread calling Polling.poll</remarks>
  /// </summary>
  [<CompiledName("Poll")>]
  let poll timeout items =
    let items  = items |> Array.ofSeq
    let items' = items |> Array.map poller
    match C.zmq_poll(items',items'.Length,timeout) with
    | 0             ->  false (* pass *)
    | n when n > 0  ->  for i in 0 .. items'.GetUpperBound(0) do
                          let e,r = items'.[i].events,items'.[i].revents
                          if e &&& r = e then items.[i] |> invoke
                        true
    | _             ->  ZMQ.error()

  /// Calls Polling.poll with the given sequence of 
  /// Poll items and 0 microseconds timeout
  [<CompiledName("PollNow")>]
  let pollNow items = poll ZMQ.NOW items

  /// Calls Polling.poll with the given sequence of Poll items and no timeout,
  /// effectively causing the polling loop to block indefinitely.
  [<CompiledName("PollForever")>]
  let pollForever items = poll ZMQ.FOREVER items

/// Utilities for working with Polling from languages other than F#
[<Extension>]
type PollingExtensions =

  /// Creates a Poll item for the socket which will 
  /// invoke the callback when the socket receives a message
  [<Extension>]
  static member AsPollIn (socket,callback:Action<_>) =
    socket |> Polling.pollIn (fun s -> callback.Invoke(s))

  /// Creates a Poll item for the socket which will 
  /// invoke the callback when the socket receives a message
  [<Extension>]
  static member AsPollOut (socket,callback:Action<_>) =
    socket |> Polling.pollOut (fun s -> callback.Invoke(s))

  /// Creates a Poll item for the socket which will 
  /// invoke the callback when the socket sends or receives a message
  [<Extension>]
  static member AsPollIO (socket,callback:Action<_>) =
    socket |> Polling.pollIO (fun s -> callback.Invoke(s))
