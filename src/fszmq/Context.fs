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
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

/// Contains methods for working with Context instances
[<Extension;CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =

(* socket types *)

  /// Creates a Socket, of the given type, within the given context 
  [<Extension;CompiledName("Socket")>]
  let newSocket (context:Context) socketType = new Socket(context.Handle,socketType)

  /// <summary>
  /// Creates a peer connected to exactly one other peer. 
  /// <para>This socket type is used primarily for inter-thread 
  /// communication across the "inproc" transport.</para>
  /// </summary>
  [<Extension;CompiledName("Pair")>]
  let pair context = newSocket context ZMQ.PAIR

  /// <summary>
  /// Creates a client for sending requests to and receiving replies from 
  /// a service. 
  /// <para>This socket type allows only an alternating sequence of 
  /// `Socket.send(request)` followed by `Socket.recv(reply)` calls.</para>
  /// </summary>
  [<Extension;CompiledName("Request")>]
  let req context = newSocket context ZMQ.REQ
 
  /// <summary>
  /// Creates a service to receive requests from and send replies to a 
  /// client. 
  /// <para>This socket type allows only an alternating sequence of 
  /// `Socket.recv(reply)` followed by `Socket.send(request)` calls.</para>
  /// </summary>
  [<Extension;CompiledName("Response")>]
  let rep context = newSocket context ZMQ.REP

  /// <summary>
  /// Creates an advanced socket type used for extending the request/reply 
  /// pattern. 
  /// <para>When a ZMQ.DEALER socket is connected to a ZMQ.REP socket,
  /// each message sent must consist of an empty message part, the 
  /// delimiter, followed by one or more body parts.</para>
  /// </summary>
  [<Extension;CompiledName("Dealer")>]
  let deal context = newSocket context ZMQ.DEALER
  
  /// <summary>
  /// Creates an advanced socket type used for extending the request/reply 
  /// pattern. 
  /// <para>When receiving messages a ZMQ.ROUTER socket prepends a 
  /// message part containing the identity of the originating peer. </para>
  /// <para>When sending messages a ZMQ.ROUTER socket removes the first 
  /// part of the message and uses it to determine the identity of the recipient.</para>
  /// </summary>
  [<Extension;CompiledName("Router")>]
  let route context = newSocket context ZMQ.ROUTER
  
  /// Creates a pipeline node to receive messages from upstream (PUSH) nodes.
  [<Extension;CompiledName("Pull")>]
  let pull context = newSocket context ZMQ.PULL
  
  /// Creates a pipeline node to send messages to downstream (PULL) nodes.
  [<Extension;CompiledName("Push")>]
  let push context = newSocket context ZMQ.PUSH
  
  /// <summary>
  /// Creates a publisher used to distribute messages to subscribers. 
  /// <para>Note: topical filtering will be done by the subscriber</para>
  /// </summary>
  [<Extension;CompiledName("Publish")>]
  let pub context = newSocket context ZMQ.PUB
  
  /// <summary>
  /// Creates a subscriber to receive to data distributed by a publisher. 
  /// <para>Initially a ZMQ.SUB socket is not subscribed to any messages 
  /// (i.e. one, or more, subscriptions must be manually applied before 
  /// any messages will be received).</para>
  /// </summary>
  [<Extension;CompiledName("Subscribe")>]
  let sub context = newSocket context ZMQ.SUB

  /// Behaves the same as a publisher, except topical filtering is done
  /// by the publisher (before sending a message)
  [<Extension;CompiledName("PublishEx")>]
  let xpub context = newSocket context ZMQ.XPUB
  
  /// <summary>
  /// Behaves the same as a subscriber, except topical filtering is done
  /// by the publisher (before sending a message). 
  /// <para>Note: subscriptions are made by sending a subscription message,
  /// in which the first byte is 1 or 0 (subscribe or unsubscribe) 
  /// and the remainder of the message is the topic</para>
  /// </summary>
  [<Extension;CompiledName("SubscribeEx")>]
  let xsub context = newSocket context ZMQ.XSUB

  /// <summary>
  /// Creates a socket which can, asynchronously, send data to or 
  /// receive data from an non-ZeroMQ peer (via the TCP transport). 
  /// <para>Note: each message should begin with a peer identity. </para>
  /// <para>Additionally, a STREAM socket can act as client or a server. 
  /// When acting as a server, the socket MUST set the SENDMORE flag. 
  /// When acting as a client, the SENDMORE flag is ignored. 
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
  let config context options =
    Seq.iter (fun (input:int * int) -> setOption context input) options 
