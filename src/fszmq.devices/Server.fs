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
open System.Threading

/// a basic router server, which runs the provided callback as a 
/// separate async workflow per-incoming-request (or per-incoming-dealer)
type Server (context  : Context,
             handler  : (byte[][] -> byte[][]),
             ?address : string, 
             ?cancel  : CancellationToken) =
  
  let mutable started = false
 
  let mutable address = defaultArg address String.Empty
  let mutable cancel  = defaultArg cancel Async.DefaultCancellationToken

  let socket = route context
  let error  = new Event<System.Exception>() 

  let getMessage() =
    let buffer  = ResizeArray()
    let hasMore = ref true
    while !hasMore do
      match tryRecv socket ZMQ.NOBLOCK with
      | Some(m) -> buffer.Add(m)
      | None    -> ()
      hasMore := socket |> recvMore
    buffer.ToArray()
  
  let runCallback (message:byte[][]) = async { 
    try
      (handler message) |> sendAll socket
    with
      | x ->  error.Trigger(x) }

  let rec runLoop (kill:CancellationToken) = async {
    if kill.IsCancellationRequested then return ()
    try
      let msg = getMessage()
      if (msg |> Array.length) > 2 then
        //  router messages *always* have at least 3 frames:
        //  one identifying the sender, a delimiter (empty frame),
        //  and one (or more (possibly empty)) frames for the body
        let! runCallback = Async.StartChild(runCallback msg)
        do!  runCallback
    with
      | x -> error.Trigger(x)
    return! runLoop kill}
  
  [<CLIEvent>]
  member __.Error = error.Publish

  member __.Address = address
  member __.Address with set v' = address <- v'

  member self.Start(?blocking:bool) =
    if started 
      then invalidOp "already started"
      else 
        started <- true
        address |> bind socket
        match (defaultArg blocking false) with
        | false -> Async.Start(runLoop cancel, cancel)
        | true  -> Async.StartImmediate(runLoop cancel, cancel)

  interface IDisposable with
    member __.Dispose() = (socket :> IDisposable).Dispose()
  
  /// starts a basic router server, bound to the given address, 
  /// which runs the provided callback as a separate async workflow 
  /// per-incoming-request (or per-incoming-dealer)
  static member Start(context   : Context,
                      handler   : (byte[][] -> byte[][]),
                      address   : string,
                      ?blocking : bool,
                      ?cancel   : CancellationToken) =
    let srv = new Server(context,handler,address,?cancel=cancel)
    srv.Start(?blocking=blocking); srv