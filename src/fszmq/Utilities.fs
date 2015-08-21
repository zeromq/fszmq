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
    |> ignore

  /// creates a proxy connection passing messages between two sockets,
  /// with an (optional) third socket for supplemental data capture,
  /// and an (optional) fourth socket for PAUSE/RESUME/TERMINATE control
  [<CompiledName("SteerableProxy")>]
  let steerableProxy  (frontend :Socket       ) 
                      (backend  :Socket       ) 
                      (capture  :Socket option) 
                      (control  :Socket option) =
    let capture,control = match capture,control with 
                          | Some p,Some t ->  p.Handle,t.Handle
                          | Some p,None   ->  p.Handle,      0n
                          | None  ,Some t ->        0n,t.Handle
                          | None  ,None   ->        0n,      0n
    C.zmq_proxy_steerable (frontend.Handle,backend.Handle,capture,control) |> ignore


/// Utilities for working with Polling from languages other than F#
[<Extension>]
type ProxyingExtensions =

  /// creates a proxy connection passing messages between two sockets
  [<Extension>]
  static member Proxy(frontend,backend) = Proxying.proxy frontend backend None

  /// creates a proxy connection passing messages between two sockets,
  /// with a third socket for supplemental data capture (e.g. logging)
  [<Extension>]
  static member Proxy(frontend,backend,capture) = Proxying.proxy frontend backend (Some capture)

  /// creates a proxy connection passing messages between two sockets,
  /// with a third socket for PAUSE/RESUME/TERMINATE control
  [<Extension>]
  static member SteerableProxy(frontend,backend,control) = 
    Proxying.steerableProxy frontend backend None (Some control)

  /// creates a proxy connection passing messages between two sockets,
  /// with a third socket for PAUSE/RESUME/TERMINATE control
  /// and a fourth socket for supplemental data capture (e.g. logging)
  [<Extension>]
  static member SteerableProxy(frontend,backend,control,capture) = 
    Proxying.steerableProxy frontend backend (Some capture) (Some control)


/// Utilities for working with ZeroMQ Base-85 Encoding
[<RequireQualifiedAccess>]
module Z85 =

  /// Encodes a binary block into a string using ZeroMQ Base-85 Encoding.
  ///
  /// ** Note: the size of the binary block MUST be divisible be 4. **
  [<CompiledName("Encode")>]
  let encode data =
    if data = null || Array.length data = 0
      then  ""
      else  let datalen = Array.length data
            let buffer  = StringBuilder (datalen * 5 / 4 + 1)
            if C.zmq_z85_encode(buffer,data,unativeint datalen) = 0n then ZMQ.error()
            string buffer

  /// Decodes ZeroMQ Base-85 encoded string to a binary block.
  ///
  /// ** Note: the size of the string MUST be divisible be 5. **
  [<CompiledName("Decode")>]
  let decode data =
    let datalen = String.length data
    if  datalen = 0 
      then  [||]
      else  let buffer  = Array.zeroCreate (datalen * 4 / 5)
            if C.zmq_z85_decode(buffer,data) = 0n then ZMQ.error()
            buffer


/// Utilities for working with the CurveZMQ security protocol
/// (NOTE: required underlying library support for CurevZMQ)
[<Experimental("WARNING: Functionality in the Curve module requires more testing.")>]
module Curve =

  let [<Literal>] private KEY_SIZE = 41 //TODO: should this be hard-coded?

  /// Returns a newly generated random keypair consisting of a public key and a secret key.
  /// The keys are encoded using ZeroMQ Base-85 Encoding.
  [<CompiledName("MakeCurveKeyPair")>]
  let curveKeyPair () =
    let publicKey,secretKey = StringBuilder(KEY_SIZE),StringBuilder(KEY_SIZE)
    if C.zmq_curve_keypair(publicKey,secretKey) <> 0 then ZMQ.error()
    (string publicKey),(string secretKey)


/// Utilities for working with Version from languages other than F#
[<Extension>]
type VersionExtensions =

  /// Executes the appropriate callback based on availability of version info
  [<Extension>]
  static member Match (value,version:Func<int,int,int,'r>,unknown:Func<'r>) =
    match value with
    | Version (m,n,b) -> version.Invoke (m,n,b)
    | Version.Unknown -> unknown.Invoke ()

  /// Executes the appropriate callback based on availability of version info
  [<Extension>]
  static member Match (value,version:Action<int,int,int>,unknown:Action) =
    match value with
    | Version (m,n,b) -> version.Invoke (m,n,b)
    | Version.Unknown -> unknown.Invoke ()

  /// Executes the given callback only if version information is available
  [<Extension>]
  static member IfKnown (value,action) =
    VersionExtensions.Match(value,action,Action (fun () -> ()))

  /// Extracts the details of the version,
  /// returning true on success and false if version info is unavailable
  [<Extension>]
  static member TryGetInfo (value ,[<Out>]major:byref<int>
                                  ,[<Out>]minor:byref<int>
                                  ,[<Out>]build:byref<int>) = 
    match value with
    | Version (m,n,b) ->  major <- m; minor <- n; build <- b;
                          true
    | Version.Unknown ->  false


/// Utilities for working with Capabiity from languages other than F#
[<Extension>]
type CapabilityExtensions =

  /// Executes the appropriate callback based on availability of capability info
  [<Extension>]
  static member Match (value,supported:Func<string,bool,'r>,unknown:Func<'r>) =
    match value with
    | Supported (name,yesOrNo)  -> supported.Invoke (name,yesOrNo)
    | Capability.Unknown        -> unknown.Invoke ()

  /// Executes the appropriate callback based on availability of capability info
  [<Extension>]
  static member Match (value,supported:Action<string,bool>,unknown:Action) =
    match value with
    | Supported (name,yesOrNo)  -> supported.Invoke (name,yesOrNo)
    | Capability.Unknown        -> unknown.Invoke ()

  /// Executes the given callback only if capability information is available
  [<Extension>]
  static member IfKnown (value,action) =
    CapabilityExtensions.Match(value,action,Action (fun () -> ()))

  /// Extracts the details of the capability,
  /// returning true on success and false if capability info is unavailable
  [<Extension>]
  static member TryGetInfo (value ,[<Out>]name    :byref<string>
                                  ,[<Out>]yesOrNo :byref<bool>) = 
    match value with
    | Supported (cap,ok)  ->  name <- cap; yesOrNo <- ok
                              true
    | Capability.Unknown  ->  false


//NOTE: This allows non-F# extensions to have proper visibility/interop with all CLR languages
[<assembly: ExtensionAttribute()>]
do()
