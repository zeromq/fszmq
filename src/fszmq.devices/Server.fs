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

open fszmq
open fszmq.Context
open fszmq.Socket
open System
open System.Runtime.CompilerServices
open System.Threading

type private message  = Data of byte[][] | Quit
type private agent    = MailboxProcessor<message>

/// a basic router server, which runs the provided callback as a 
/// separate async workflow per-incoming-request (or per-incoming-dealer)
type Server (context  : Context,
             handler  : (CancellationToken -> byte[][] -> byte[][]),
             ?address : string) =
  
  let mutable started = false
  let mutable address = defaultArg address String.Empty

  let cancel = Async.DefaultCancellationToken
  let socket = route context
  let error  = new Event<System.Exception>() 

  let getMessage () =
    try
      let buffer  = ResizeArray()
      let hasMore = ref true
      while !hasMore do
        match tryRecv socket ZMQ.NOBLOCK with
        | Some(m) -> buffer.Add(m)
        | None    -> ()
        hasMore := socket |> recvMore
      buffer.ToArray()
    with
      | x -> raise x
  
  let server = new agent (fun inbox -> 
    
    let callback msg = 
      async { try
                inbox.Post (Data (handler cancel msg)) 
              with 
                | x -> error.Trigger x }
    
    let rec loop ()  = 
      async { let raw = getMessage ()
              if (raw |> Array.length) > 2 then Async.Start (callback raw)
              let!  msg = inbox.TryReceive 2000
              match msg with
              | Some(m) ->  match m with
                            | Data(d) ->  d |> sendAll socket
                                          return! loop ()
                            | Quit    ->  Async.CancelDefaultToken ()
                                          return () // exit
              | None    ->  return! loop () }
    
    loop ())
  
  [<CLIEvent>]
  member __.Error = error.Publish

  member __.Address = address
  member __.Address with set v' = address <- v'

  /// starts a basic router server, binding to the Address property
  member self.Start () =
    if started 
      then invalidOp "already started"
      else 
        started <- true
        address |> bind socket
        server.Error.Add error.Trigger
        server.Start ()

  interface IDisposable with
    member self.Dispose() = 
      server.Post Quit
      (server :> IDisposable).Dispose ()
      (socket :> IDisposable).Dispose ()
  
  /// starts a basic router server, bound to the given address, 
  /// which runs the provided callback as a separate async workflow 
  /// per-incoming-request (or per-incoming-dealer)
  static member Start(context,handler,address) =
    let srv = new Server(context,handler,address)
    srv.Start(); srv


/// contains methods for working with ZMQ Server instances
[<Extension>]
type Interop =

  static member private toFSFunc(handler:Func<_,_,_>) = 
    (fun a b -> handler.Invoke(a,b))
    
  /// starts a basic router server, bound to the given address,
  /// which runs the provided callback as a separate async workflow 
  /// per-incoming-request (or incoming per-incoming-dealer)
  [<Extension>]  
  static member StartServer(context,address,handler) =
    let handler' = Interop.toFSFunc(handler)
    Server.Start(context,handler',address)
