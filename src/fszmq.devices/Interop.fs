(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace fszmq.devices

open Microsoft.FSharp.Core
open fszmq
open System
open System.Runtime.CompilerServices
open System.Threading

/// contains methods for working with ZMQ Server instances
[<Extension>]
type Interop =

  static member private toFSFunc(handler:Func<_,_>) = 
    (fun a -> handler.Invoke(a))

  /// starts a basic router server, bound to the given address, 
  /// interruptible via the given cancellation token, 
  /// which runs the provided callback as a separate async workflow 
  /// per-incoming-request (or per-incoming-dealer). 
  /// if blocking is true, the server will run on the current thread; 
  /// otherwise, it will run on background thread.
  [<Extension>]  
  static member StartServer(context,address,handler,blocking,cancel) =
    let handler' = Interop.toFSFunc(handler)
    Server.Start(context,handler',address,blocking,cancel)
    
  /// starts a basic router server, bound to the given address, 
  /// which runs the provided callback as a separate async workflow 
  /// per-incoming-request (or per-incoming-dealer).
  /// if blocking is true, the server will run on the current thread; 
  /// otherwise, it will run on background thread.
  [<Extension>]  
  static member StartServer(context,address,handler,blocking) =
    let handler' = Interop.toFSFunc(handler)
    Server.Start(context,handler',address,blocking)

  /// starts a basic router server, bound to the given address,
  /// interruptible via the given cancellation token, which runs the 
  /// provided callback as a separate async workflow 
  /// per-incoming-request (or incoming per-incoming-dealer)
  [<Extension>]  
  static member StartServer(context,address,handler,cancel) =
    let handler' = Interop.toFSFunc(handler)
    Server.Start(context,handler',address,?cancel=Some(cancel))

  /// starts a basic router server, bound to the given address,
  /// which runs the provided callback as a separate async workflow 
  /// per-incoming-request (or incoming per-incoming-dealer)
  [<Extension>]  
  static member StartServer(context,address,handler) =
    let handler' = Interop.toFSFunc(handler)
    Server.Start(context,handler',address)
