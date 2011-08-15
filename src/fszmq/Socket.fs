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

open System.Runtime.CompilerServices
open System.Runtime.InteropServices

/// contains methods for working with ZMQ Socket instances
[<Extension>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Socket =

  /// initializes an endpoint for accepting connections
  [<Extension;CompiledName("Bind")>]
  let bind (socket:Socket) address = 
    let okay = C.zmq_bind(socket.Handle,address) 
    if okay <> 0 then raise <| ZeroMQException()

  /// connects to an endpoint at the given address
  [<Extension;CompiledName("Connect")>]
  let connect (socket:Socket) address = 
    let okay = C.zmq_connect(socket.Handle,address) 
    if okay <> 0 then raise <| ZeroMQException()


(* option getting/setting *)

  /// retrieves the value of the given ZMQ socket option
  [<Extension;CompiledName("GetOption")>]
  let get<'t> (socket:Socket) key : 't =
    
    let mutable size' = 
      match typeof<'t> with
      | Binary -> 255un // maximum length for a binary option
      | Bool   // in ZeroMQ, boolean value are 64-bit integers (0 = false)
      | ULong  // UInt64 occupies the same number of bytes as an Int64
      | Long   -> 8un 
      | Int    -> 4un 
      | _      -> invalidOp "argument 'value' has an invalid data type"

    use mem = nativeMem size'
    let okay = C.zmq_getsockopt(socket.Handle,key,mem.Handle,&size')
    if okay <> 0 then raise <| ZeroMQException()
    
    downcast (
      match typeof<'t> with
      | Binary -> (extractNative size' >> box) mem
      | Bool   -> (Marshal.ReadInt64 mem.Handle) |> (int64 >> bool >> box)
      | ULong  -> (Marshal.ReadInt64 mem.Handle) |> (uint64 >> box)
      | Long   -> (Marshal.ReadInt64 mem.Handle) |> box
      | Int    -> (Marshal.ReadInt32 mem.Handle) |> box
      | _      -> invalidOp "argument 'value' has an invalid data type")

  /// assigns a value to the given ZMQ socket option
  [<Extension;CompiledName("SetOption")>]
  let set (socket:Socket) (key,value:'t) =
    
    use mem = 
      match (box value) with
      | :?  int32   as v -> (nativeMem 4un) |> writeNative32 v
      | :?  int64   as v -> (nativeMem 8un) |> writeNative64 v
      | :? uint64   as v -> (nativeMem 8un) |> writeNative64 (long v)
      | :?  bool    as v -> (nativeMem 8un) |> writeNative64 (long v)
      | :? (byte[]) as v -> (nativeMem v.Length) |> fillNative v
      | _ -> invalidOp "argument 'value' has an invalid data type"
    
    let okay = C.zmq_setsockopt(socket.Handle,key,mem.Handle,mem.Size) 
    if okay <> 0 then raise <| ZeroMQException()

  /// assigns a value to the given ZMQ socket option, 
  /// returning updated socket instance
  [<CompiledName("ApplyOption")>]
  let apply input (socket:Socket) = input |> set socket; socket

  [<Extension;CompiledName("SetOptions")>]
  let config (socket:Socket) (options:seq<int * obj>) =
    let set' = set socket
    options |> Seq.iter (fun input -> set' input)

 
(* message sending *)

  let private (|Okay|Busy|Fail|) = function 
    | 0 ->  Okay 
    | _ ->  match C.zmq_errno() with 
            | C.EAGAIN  -> Busy
            | _         -> Fail

  /// sends a message part, with the given flags, returning true 
  /// if successful or false if the transmission should be re-tried
  [<Extension;CompiledName("TrySend")>]
  let trySend (socket:Socket) flags message =
    use msg = new Message(message)
    match C.zmq_send(socket.Handle,msg.Handle,flags) with
    | Okay -> true
    | Busy -> false
    | Fail -> raise <| ZeroMQException()

  let private waitToSend socket flags message =
    let rec send' ()  =
      match trySend socket flags message with
      | true  -> ((* okay *))
      | false -> send'()
    send'()

  /// sends a message part, indicating no more parts will follow
  [<Extension;CompiledName("Send")>]
  let send socket message = 
    message |> waitToSend socket ZMQ.BLOCK
  
  /// sends a message part, indicating more parts will follow
  [<Extension;CompiledName("SendMore")>]
  let sendMore (socket:Socket) message = 
    message |> waitToSend socket ZMQ.SNDMORE
    socket
  
  /// alias for 'Socket.send'
  let (<<|) socket = send socket
  /// alias for 'Socket.sendMore'
  let (<~|) socket = sendMore socket

  /// alias for 'Socket.send'
  let (|>>) data socket = socket <<| data
  /// alias for 'Socket.sendMore'
  let (|~>) data socket = socket <~| data

  /// sends all parts of a given message
  [<Extension;CompiledName("SendAll")>]
  let sendAll (socket:Socket) (message:#seq<_>) =
    let last = (message |> Seq.length) - 1
    message 
      |> Seq.mapi (fun i msg -> 
                    if i = last then (|>>) msg else ((|~>) msg) >> ignore)
      |> Seq.iter (fun send' -> socket |> send')


(* message receiving *)

  /// retrieves the next available message part from a socket, returning 
  /// Some(<message>) if successful or None if it should be re-attempted
  [<Extension;CompiledName("TryRecv")>]
  let tryRecv (socket:Socket) flags =
    use msg = new Message()
    match C.zmq_recv(socket.Handle,msg.Handle,flags) with
    | Okay -> let mutable frame = Array.empty
              frame <- msg.Data
              Some(frame)
    | Busy -> None
    | Fail -> raise <| ZeroMQException()

  /// retrieves the next available message part from a socket
  [<Extension;CompiledName("Recv")>]
  let recv (socket:Socket) =
    use msg = new Message()
    match C.zmq_recv(socket.Handle,msg.Handle,ZMQ.BLOCK) with
    | Okay -> let mutable message = Array.empty
              message <- msg.Data
              message
    | _ -> raise <| ZeroMQException()
    
  /// returns true if more message parts are available, false otherwise
  [<Extension;CompiledName("HasMore")>]
  let recvMore socket = get<bool> socket ZMQ.RCVMORE

  /// retrieves all parts of the next available message
  [<Extension;CompiledName("RecvAll")>]
  let recvAll socket =
    [|  yield socket |> recv 
        while socket |> recvMore do yield socket |> recv  |]
  
  /// copies a message part-wise from one socket to another without
  /// first marshalling the message part into the managed code space
  [<Extension;CompiledName("Transfer")>]
  let transfer (socket:Socket) (target:Socket) =
    use msg = new Message()
    let rec send' flags =
      match C.zmq_send(target.Handle,msg.Handle,flags) with
      | Okay -> ((* pass *))
      | Busy -> send' flags
      | Fail -> raise <| ZeroMQException()
    let loop = ref true
    while !loop do
      match C.zmq_recv(socket.Handle,msg.Handle,ZMQ.BLOCK) with
      | Okay -> loop := socket |> recvMore
                send' (if !loop then ZMQ.SNDMORE else ZMQ.NOBLOCK)
                //TODO: maybe change previous call to ZMQ.BLOCK?
      | _ -> raise <| ZeroMQException()

  /// alias for 'Socket.transfer'
  let (>|<) socket target = target |> transfer socket


(* subscription handling *)

  /// adds one subscription for each of the given topics
  [<Extension;CompiledName("Subscribe")>]
  let subscribe socket topics =
    let setter (t:byte[]) = set socket (ZMQ.SUBSCRIBE,t) |> ignore
    topics |> Seq.iter setter

  /// removes one subscription for each of the given topics
  [<Extension;CompiledName("Unsubscribe")>]
  let unsubscribe socket topics =
    let setter (t:byte[]) = set socket (ZMQ.UNSUBSCRIBE,t) |> ignore
    topics |> Seq.iter setter
