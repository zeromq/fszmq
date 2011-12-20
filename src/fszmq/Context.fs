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
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

/// Contains methods for working with `Context` instances
[<Extension>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =

  /// Creates a `Socket` within the specified context 
  [<Extension;CompiledName("Socket")>]
  let newSocket (context:Context) socketType = 
    new Socket(context.Handle,socketType)

  /// <summary>
  /// Creates a peer connected to exactly one other peer.
  /// <para>This socket type is used primarily for inter-thread 
  /// communication across the "inproc" transport.</para>
  /// </summary>
  [<Extension;CompiledName("Pair")>]
  let pair (context:Context) = ZMQ.PAIR |> newSocket context
  
  /// <summary>
  /// Creates a client for sending requests to and receiving replies from 
  /// a service.
  /// <para>This socket type allows only an alternating sequence of 
  /// `Socket.send(request)` followed by `Socket.recv(reply)` calls.</para>
  /// </summary>
  [<Extension;CompiledName("Request")>]
  let req (context:Context) = ZMQ.REQ |> newSocket context
 
  /// <summary>
  /// Creates a service to receive requests from and send replies to a 
  /// client.
  /// <para>This socket type allows only an alternating sequence of 
  /// `Socket.recv(reply)` followed by `Socket.send(request)` calls.</para>
  /// </summary>
  [<Extension;CompiledName("Response")>]
  let rep (context:Context) = ZMQ.REP |> newSocket context
  
  /// <summary>
  /// Creates an advanced socket type used for extending the request/reply 
  /// pattern.
  /// <para>When a ZMQ.DEALER socket is connected to a ZMQ.REP socket,
  /// each message sent must consist of an empty message part, the 
  /// delimiter, followed by one or more body parts.</para>
  /// </summary>
  [<Extension;CompiledName("Dealer")>]
  let deal (context:Context) = ZMQ.DEALER |> newSocket context
  
  /// <summary>
  /// Creates an advanced socket type used for extending the request/reply 
  /// pattern. 
  /// <para>When receiving messages a ZMQ.ROUTER socket prepends a 
  /// message part containing the identity of the originating peer.</para>
  /// <para>When sending messages a ZMQ.ROUTER socket removes the first 
  /// part of the message and uses it to determine the identity of 
  /// the recipient.</para>
  /// </summary>
  [<Extension;CompiledName("Router")>]
  let route (context:Context) = ZMQ.ROUTER |> newSocket context
  
  /// Creates a pipeline node to receive messages from upstream (PUSH) nodes.
  [<Extension;CompiledName("Pull")>]
  let pull (context:Context) = ZMQ.PULL |> newSocket context
  
  /// Creates a pipeline node to send messages to downstream (PULL) nodes.
  [<Extension;CompiledName("Push")>]
  let push (context:Context) = ZMQ.PUSH |> newSocket context
  
  /// Creates a publisher used to distribute messages to subscribers.
  [<Extension;CompiledName("Publish")>]
  let pub (context:Context) = ZMQ.PUB |> newSocket context
  
  /// <summary>
  /// Creates a subscriber to receive to data distributed by a publisher.
  /// <para>Initially a ZMQ.SUB socket is not subscribed to any messages 
  /// (i.e. one, or more, subscriptions must be manually applied before 
  /// any messages will be received).</para>
  /// </summary>
  [<Extension;CompiledName("Subscribe")>]
  let sub (context:Context) = ZMQ.SUB |> newSocket context
