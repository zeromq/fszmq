(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)
namespace fszmq

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

/// Contains methods for working with Context instances
[<Extension;CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =

(* socket types *)

  /// Creates a Socket, of the given type, within the given context
  [<Extension;CompiledName("Socket")>]
  let newSocket (context:Context) socketType =
    let socket = new Socket (context.Handle,socketType)
    context.Attach socket
    socket

  /// <summary>
  /// Creates a peer connected to exactly one other peer.
  /// <para> This socket type is used primarily for inter-thread
  /// communication across the "inproc" transport.</para>
  /// </summary>
  [<Extension;CompiledName("Pair")>]
  let pair context = newSocket context ZMQ.PAIR

  /// <summary>
  /// Creates a client for sending requests to and receiving replies from
  /// a service.
  /// <para> This socket type allows only an alternating sequence of
  /// `Socket.send(request)` followed by `Socket.recv(reply)` calls.</para>
  /// </summary>
  [<Extension;CompiledName("Req")>]
  let req context = newSocket context ZMQ.REQ

  /// <summary>
  /// Creates a service to receive requests from and send replies to a
  /// client.
  /// <para> This socket type allows only an alternating sequence of
  /// `Socket.recv(reply)` followed by `Socket.send(request)` calls.</para>
  /// </summary>
  [<Extension;CompiledName("Rep")>]
  let rep context = newSocket context ZMQ.REP

  /// <summary>
  /// Creates an advanced socket type used for extending the request/reply
  /// pattern.
  /// <para> When a `ZMQ.DEALER` socket is connected to a `ZMQ.REP` socket,
  /// each message sent must consist of an empty message part, the
  /// delimiter, followed by one or more body parts.</para>
  /// </summary>
  [<Extension;CompiledName("Dealer")>]
  let dealer context = newSocket context ZMQ.DEALER

  /// <summary>
  /// Creates an advanced socket type used for extending the request/reply
  /// pattern.
  /// <para> When receiving messages a `ZMQ.ROUTER` socket prepends a
  /// message part containing the identity of the originating peer. </para>
  /// <para> When sending messages a `ZMQ.ROUTER` socket removes the first
  /// part of the message and uses it to determine the identity of the recipient.</para>
  /// </summary>
  [<Extension;CompiledName("Router")>]
  let router context = newSocket context ZMQ.ROUTER

  /// Creates a pipeline node to receive messages from upstream (`ZMQ.PUSH`) nodes.
  [<Extension;CompiledName("Pull")>]
  let pull context = newSocket context ZMQ.PULL

  /// Creates a pipeline node to send messages to downstream (`ZMQ.PULL`) nodes.
  [<Extension;CompiledName("Push")>]
  let push context = newSocket context ZMQ.PUSH

  /// <summary>
  /// Creates a publisher used to distribute messages to subscribers.
  ///
  /// **Note: topical filtering will be done at the subscriber (after receiving messages)**
  /// </summary>
  [<Extension;CompiledName("Pub")>]
  let pub context = newSocket context ZMQ.PUB

  /// <summary>
  /// Creates a subscriber to receive to data distributed by a publisher.
  /// <para> Initially a `ZMQ.SUB` socket is not subscribed to any messages
  /// (i.e. one, or more, subscriptions must be applied, via `Socket.setOption`,
  /// before any messages will be received).</para>
  /// </summary>
  [<Extension;CompiledName("Sub")>]
  let sub context = newSocket context ZMQ.SUB

  /// Behaves the same as a publisher, except this socket type may also receive
  /// subscription messages from peers.
  ///
  /// **Note: topical filtering will be done at the publisher (before sending messages)**
  [<Extension;CompiledName("XPub")>]
  let xpub context = newSocket context ZMQ.XPUB

  /// <summary>
  /// Behaves the same as a subscriber, except topical filtering is done
  /// by sending subscription messages to the publisher.
  /// <para> Subscriptions are made by sending a subscription message,
  /// in which the first byte is 1 or 0 (subscribe or unsubscribe)
  /// and the remainder of the message is the topic</para>
  /// </summary>
  [<Extension;CompiledName("XSub")>]
  let xsub context = newSocket context ZMQ.XSUB

  /// <summary>
  /// Creates a socket which can, asynchronously, send data to or
  /// receive data from an non-ZeroMQ peer (via the "TCP" transport).
  /// <para> Note: each message should begin with a peer identity. </para>
  /// <para> Additionally, a `ZMQ.STREAM` socket can act as client or a server.
  /// When acting as a server, the socket MUST set the `ZMQ.SENDMORE` flag.
  /// When acting as a client, the `ZMQ.SENDMORE` flag is ignored.
  /// Sending an identity followed by an empty frame, closes the connection.</para>
  /// </summary>
  [<Extension;CompiledName("Stream")>]
  let stream context = newSocket context ZMQ.STREAM

(* context options *)

  /// Gets the value of the given option for the given Context
  [<Extension;CompiledName("GetOption")>]
  let getOption (context:Context) contextOption =
    match C.zmq_ctx_get(context.Handle,contextOption) with
    |    -1 -> ZMQ.error()
    | value -> value

   /// Sets the given option value for the given Context
  [<Extension;CompiledName("SetOption")>]
  let setOption (context:Context) (contextOption,value) =
    if C.zmq_ctx_set(context.Handle,contextOption,value) <> 0 then ZMQ.error()

  /// Sets the given block of option values for the given Context
  [<Extension;CompiledName("Configure")>]
  let configure context options =
    Seq.iter (fun (input:int * int) -> setOption context input) options
