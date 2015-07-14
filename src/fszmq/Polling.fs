(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)
namespace fszmq

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

/// For use with the Polling module...
///
/// Associates a callback with a Socket instance and one or more events,
/// such that the callback is invoked when the event(s) occurs on the Socket instance
///
/// ** Note: all sockets passed to Polling.poll MUST share the same context
/// and belong to the thread calling Polling.poll **
type Poll = 
  /// Creates a new poll item, associating the given events, socket, and callback
  | Poll of events:int16 * socket:Socket * callback:(Socket -> unit) with

  /// Creates a poll item in a way friendly to languages other then F#
  static member Create(events,socket,callback:Action<Socket>) = Poll(events,socket,fun s -> callback.Invoke(s))

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
  ///
  [<CompiledName("PollIO")>]
  let pollIO fn socket = Poll(ZMQ.POLLIN ||| ZMQ.POLLOUT,socket,fn)

  /// Performs a single polling run
  /// across the given sequence of Poll items, waiting up to the given timeout.
  /// Returns true when one or more callbacks have been invoked, returns false otherwise.
  ///
  /// ** Note: All items passed to Polling.poll MUST share the same context
  /// and belong to the thread calling `Polling.poll`. **
  ///
  /// This function is named DoPoll in compiled assemblies.
  /// If you are accessing the function from a language other than F#, or through reflection, use this name.
  [<CompiledName("DoPoll")>]
  let poll<[<Measure>]'unit> (timeout:int64<'unit>) items =
    let items  = items |> Array.ofSeq
    let items' = items |> Array.map (fun (Poll(v,s,_)) -> C.zmq_pollitem_t(s.Handle,v))
    match C.zmq_poll(items',items'.Length,int64 timeout) with
    | 0             ->  false (* pass *)
    | n when n > 0  ->  for i in 0 .. items'.GetUpperBound(0) do
                          let e,r = items'.[i].events
                                   ,items'.[i].revents
                          if e &&& r = e then items.[i] |> (fun (Poll(_,s,f)) -> f s)
                        true
    | _             ->  ZMQ.error()

  /// Calls `Polling.poll` with the given sequence of Poll items and 0 microseconds timeout
  [<CompiledName("PollNow")>]
  let pollNow items = poll ZMQ.NOW items

  /// Calls `Polling.poll` with the given sequence of Poll items and no timeout,
  /// effectively causing the polling loop to block indefinitely.
  [<CompiledName("PollForever")>]
  let pollForever items = poll ZMQ.FOREVER items

  /// Polls the given socket, up to the given timeout, for an input message.
  /// Returns a byte[][] option, where None indicates no message was received.
  [<CompiledName("TryPollInput")>]
  let tryPollInput<[<Measure>]'unit> (timeout:int64<'unit>) socket =
    let msg   = ref Array.empty
    let items = [socket |> pollIn (Socket.recvAll >> ((:=) msg))]
    match poll timeout items with
    | true  -> Some !msg
    | false -> None


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

  /// Polls the given socket, up to the given timeout, for an input message.
  /// Retuns true if input was received, in which case the message is assigned to the out parameter.
  [<Extension>]
  static member TryGetInput (socket,timeout:int64,[<Out>]message:byref<byte[][]>) =
    match Polling.tryPollInput timeout socket with
    | Some msg  -> message <- msg;  true
    | None      -> message <- [||]; false
