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
open System.Runtime.CompilerServices

/// Contains methods for working with ZMQ's proxying capabilities
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Proxying =

  /// creates a proxy connection passing messages between two sockets, 
  /// with an (optional) third socket for supplemental data capture 
  [<CompiledName("Proxy")>]
  let proxy (frontend:Socket) (backend:Socket) (capture:Socket option) =
    match capture with
    | Some(capture) -> C.zmq_proxy(!!frontend,!!backend,!!capture)
    | _             -> C.zmq_proxy(!!frontend,!!backend,       0n)

/// Utilities for working with Polling from languages other than F#
[<Extension>]
type ProxyingExtensions =
  
  /// creates a proxy connection passing messages between two sockets
  [<Extension>]
  static member Proxy(frontend,backend) = 
    Proxying.proxy frontend backend None

  /// creates a proxy connection passing messages between two sockets, 
  /// with an third socket for supplemental data capture (e.g. logging)
  [<Extension>]
  static member Proxy(frontend,backend,capture) = 
    Proxying.proxy frontend backend (Some capture)
