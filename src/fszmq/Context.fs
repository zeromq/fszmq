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

/// contains methods for working with ZMQ Context instances
[<Extension>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =

  /// creates a ZMQ socket within the specified context 
  [<Extension;CompiledName("Socket")>]
  let newSocket (context:Context) socketType = 
    new Socket(context.Handle,socketType)

  /// creates a peer connected to exactly one other peer; this socket type
  /// is used for inter-thread communication across the "inproc" transport
  [<Extension;CompiledName("Pair")>]
  let pair (context:Context) = ZMQ.PAIR |> newSocket context
  
  /// create a client to send requests to and receive replies from a 
  /// service; this socket type allows only an alternating sequence of 
  /// 'send(request)' and subsequent 'recv(reply)' calls
  [<Extension;CompiledName("Request")>]
  let req (context:Context) = ZMQ.REQ |> newSocket context
 
  /// creates a service to receive requests from and send replies to a 
  /// client; this socket type allows only an alternating sequence of 
  /// 'recv(request)' and 'send(reply)' calls
  [<Extension;CompiledName("Response")>]
  let rep (context:Context) = ZMQ.REP |> newSocket context
  
  /// creates an advanced socket type used for extending the request/reply 
  /// pattern; when a ZMQ.DEALER socket is connected to a ZMQ.REP socket,
  /// each message sent must consist of an empty message part, the 
  /// delimiter, followed by one or more body parts
  [<Extension;CompiledName("Dealer")>]
  let deal (context:Context) = ZMQ.DEALER |> newSocket context
  
  /// creates an advanced socket type used for extending the request/reply 
  /// pattern; when receiving messages a ZMQ.ROUTER socket prepends a 
  /// message part containing the identity of the originating peer; 
  /// when sending messages a ZMQ.ROUTER socket removes the first part of 
  /// the message and uses it to determine the identity of the recipient
  [<Extension;CompiledName("Router")>]
  let route (context:Context) = ZMQ.ROUTER |> newSocket context
  
  /// creates a pipeline node to receive messages from upstream pipeline nodes
  [<Extension;CompiledName("Pull")>]
  let pull (context:Context) = ZMQ.PULL |> newSocket context
  
  /// creates a pipeline node to send messages to downstream pipeline nodes
  [<Extension;CompiledName("Push")>]
  let push (context:Context) = ZMQ.PUSH |> newSocket context
  
  /// creates a publisher used to distribute messages to subscribers
  [<Extension;CompiledName("Publish")>]
  let pub (context:Context) = ZMQ.PUB |> newSocket context
  
  /// creates a subscriber to receive to data distributed by a publisher;
  /// initially a ZMQ.SUB socket is not subscribed to any messages (i.e.
  /// one, or more, subscriptions must be manually applied before any 
  /// messages will be received)
  [<Extension;CompiledName("Subscribe")>]
  let sub (context:Context) = ZMQ.SUB |> newSocket context
