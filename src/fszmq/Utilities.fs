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
open System.Text

/// Contains methods for working with ZMQ's proxying capabilities
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Proxying =

  /// creates a proxy connection passing messages between two sockets, 
  /// with an (optional) third socket for supplemental data capture 
  [<CompiledName("Proxy")>]
  let proxy (frontend:Socket) (backend:Socket) (capture:Socket option) =
    match capture with
    | Some capture -> C.zmq_proxy(frontend.Handle,backend.Handle,capture.Handle)
    | _            -> C.zmq_proxy(frontend.Handle,backend.Handle,            0n)

/// Utilities for working with Polling from languages other than F#
[<Extension>]
type ProxyingExtensions =
  
  /// creates a proxy connection passing messages between two sockets
  [<Extension>]
  static member Proxy(frontend,backend) = Proxying.proxy frontend backend None

  /// creates a proxy connection passing messages between two sockets, 
  /// with an third socket for supplemental data capture (e.g. logging)
  [<Extension>]
  static member Proxy(frontend,backend,capture) = Proxying.proxy frontend backend (Some capture)

[<RequireQualifiedAccess>]
module Z85 =
    
  let [<Literal>] private KEY_SIZE = 41 //TODO: should this be hard-coded?

  [<CompiledName("MakeCurveKeyPair")>]
  let curveKeyPair () = 
    let publicKey,secretKey = StringBuilder(KEY_SIZE),StringBuilder(KEY_SIZE)
    if C.zmq_curve_keypair(publicKey,secretKey) <> 0 then ZMQ.error()
    publicKey,secretKey

  [<CompiledName("Encode")>]
  let encode data =
    let datalen = Array.length data // size must be divisible by 4
    let buffer  = StringBuilder (int ((float datalen * 1.25) + 1.0))
    C.zmq_z85_encode(buffer,data,unativeint datalen) |> ignore
    string buffer

  [<CompiledName("Encode")>]
  let decode data =
    let datalen = String.length data // size must be divisible by 5
    let buffer  = Array.zeroCreate (int (float datalen * 0.8))
    C.zmq_z85_decode(buffer,data) |> ignore
    buffer
