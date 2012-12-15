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

/// Contains methods for working with Socket instances
[<Extension;
  CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Socket =

(* connectivity *)

  /// Causes an endpoint to start accepting
  /// connections at the given address
  [<Extension;CompiledName("Bind")>]
  let bind (socket:Socket) address =
    let okay = C.zmq_bind(socket.Handle,address)
    if  okay <> 0 then ZMQ.error()

  /// Causes an endpoint to stop accepting
  /// connections at the given address
  [<Extension;CompiledName("Unbind")>]
  let unbind (socket:Socket) address =
    let okay = C.zmq_unbind(socket.Handle,address)
    if  okay <> 0 then ZMQ.error()

  /// Connects to an endpoint to the given address
  [<Extension;CompiledName("Connect")>]
  let connect (socket:Socket) address =
    let okay = C.zmq_connect(socket.Handle,address)
    if  okay <> 0 then ZMQ.error()

  /// Disconnects to an endpoint from the given address
  [<Extension;CompiledName("Disconnect")>]
  let disconnect (socket:Socket) address =
    let okay = C.zmq_disconnect(socket.Handle,address)
    if  okay <> 0 then ZMQ.error()

(* socket options *)

  /// Gets the value of the given option for the given Socket
  [<Extension;CompiledName("GetOption")>]
  let get<'t> (socket:Socket) socketOption : 't =
    let size,read =
      let   t = typeof<'t>
      if    t = typeof<int>     then   4,(snd >> readInt32  >> box)
      elif  t = typeof<bool>    then   4,(snd >> readBool   >> box)
      elif  t = typeof<int64>   then   8,(snd >> readInt64  >> box)
      elif  t = typeof<uint64>  then   8,(snd >> readUInt64 >> box)
      elif  t = typeof<string>  then 255,(       readString >> box)
      elif  t = typeof<byte[]>  then 255,(       readBytes  >> box)
                                else invalidOp "Invalid data type"
    let buffer = Marshal.AllocHGlobal(size)
    try
      let mutable size' = unativeint size
      let okay = C.zmq_getsockopt(socket.Handle,socketOption,buffer,&size')
      if  okay <> 0 then ZMQ.error()
      downcast read (size',buffer)
    finally
      Marshal.FreeHGlobal(buffer)

  /// Sets the given option value for the given Socket
  [<Extension;CompiledName("SetOption")>]
  let set (socket:Socket) (socketOption,value:'t) =
    let size,write =
      match box value with
      | :? (int32 ) as v  -> sizeof<Int32>,(writeInt32  v)     
      | :? (bool  ) as v  -> sizeof<Int32>,(writeBool   v)   
      | :? (int64 ) as v  -> sizeof<Int32>,(writeInt64  v)    
      | :? (uint64) as v  -> sizeof<Int64>,(writeUInt64 v)   
      | :? (string) as v  -> v.Length     ,(writeString v)
      | :? (byte[]) as v  -> v.Length     ,(writeBytes  v)
      | _                 -> invalidOp "Invalid data type"
    let buffer = Marshal.AllocHGlobal(size)
    try
      write(buffer)
      let okay = C.zmq_setsockopt(socket.Handle
                                 ,socketOption
                                 ,buffer
                                 ,unativeint size)
      if  okay <> 0 then ZMQ.error()
    finally
      Marshal.FreeHGlobal(buffer)

  /// Sets the given block of option values for the given Socket
  [<Extension;CompiledName("Configure")>]
  let config socket (socketOptions: (int * obj) seq) =
    let set' = set socket in socketOptions |> Seq.iter (set')
  
(* subscripitions *)

  /// Adds one subscription for each of the given topics
  [<Extension;CompiledName("Subscribe")>]
  let subscribe socket topics =
    let setter (t:byte[]) = set socket (ZMQ.SUBSCRIBE,t) |> ignore
    topics |> Seq.iter setter

  /// Removes one subscription for each of the given topics
  [<Extension;CompiledName("Unsubscribe")>]
  let unsubscribe socket topics =
    let setter (t:byte[]) = set socket (ZMQ.UNSUBSCRIBE,t) |> ignore
    topics |> Seq.iter setter

(* message sending *)
  let private (|Okay|Busy|Fail|) = function 
    | -1 -> match C.zmq_errno() with 
            | ZMQ.EAGAIN  -> Busy
            | _           -> Fail
    | _  ->  Okay 

  /// Sends a frame, with the given flags, returning true (or false) 
  /// if the send was successful (or should be re-tried)
  [<Extension;CompiledName("TrySend")>]
  let trySend (socket:Socket) flags frame =
    use frm = new Frame(frame)
    match C.zmq_msg_send(frm.Handle,socket.Handle,flags) with
    | Okay -> true
    | Busy -> false
    | Fail -> ZMQ.error()

  let private waitToSend socket flags frame =
    let rec send' ()  =
      match trySend socket flags frame with
      | true  -> ((* okay *))
      | false -> send'()
    send'()

  /// Sends a frame, indicating no more frames will follow
  [<Extension;CompiledName("Send")>]
  let send socket frame = 
    frame |> waitToSend socket ZMQ.WAIT
  
  /// Sends a frame, indicating more frames will follow, 
  /// and returning the given socket
  [<Extension;CompiledName("SendMore")>]
  let sendMore (socket:Socket) frame = 
    frame |> waitToSend socket ZMQ.SNDMORE
    socket
  
  /// Operator equivalent to Socket.send
  let (<<|) socket = send socket
  /// Operator equivalent to Socket.sendMore
  let (<~|) socket = sendMore socket

  /// Operator equivalent to Socket.send (with arguments reversed)
  let (|>>) data socket = socket <<| data
  /// Operator equivalent to Socket.sendMore (with arguments reversed)
  let (|~>) data socket = socket <~| data

  /// Sends all frames of a given message
  [<Extension;CompiledName("SendAll")>]
  let sendAll (socket:Socket) (message:#seq<_>) =
    let last = (message |> Seq.length) - 1
    message 
    |> Seq.mapi (fun i msg -> if i = last then ((|>>) msg) 
                                          else ((|~>) msg) >> ignore)
    |> Seq.iter (fun send' -> socket |> send')

(* message receiving *)

  /// Gets the next available frame from a socket, returning option<frame> 
  /// where None indicates the operation should be re-attempted
  [<Extension;CompiledName("TryRecv")>]
  let tryRecv (socket:Socket) flags =
    use frm = new Frame()
    match C.zmq_msg_recv(frm.Handle,socket.Handle,flags) with
    | Okay -> let mutable frame = Array.empty
              frame <- frm.Data
              Some(frame)
    | Busy -> None
    | Fail -> ZMQ.error()

  /// Waits for (and returns) the next available frame from a socket
  [<Extension;CompiledName("Recv")>]
  let recv socket = Option.get (tryRecv socket ZMQ.WAIT)
  
  /// Returns true if more message frames are available
  [<Extension;CompiledName("RecvMore")>]
  let recvMore socket = get<bool> socket ZMQ.RCVMORE

  /// Retrieves all frames of the next available message
  [<Extension;CompiledName("RecvAll")>]
  let recvAll socket =
    [|  yield socket |> recv 
        while socket |> recvMore do yield socket |> recv  |]
  
  /// Copies a message frame-wise from one socket to another without
  /// first marshalling the message part into the managed code space
  [<Extension;CompiledName("Transfer")>]
  let transfer (socket:Socket) (target:Socket) =
    use frm = new Frame()
    let rec send' flags =
      match C.zmq_msg_send(frm.Handle,target.Handle,flags) with
      | Okay -> ((* pass *))
      | Busy -> send' flags
      | Fail -> ZMQ.error()
    let loop = ref true
    while !loop do
      match C.zmq_msg_recv(frm.Handle,socket.Handle,ZMQ.WAIT) with
      | Okay -> loop := socket |> recvMore
                send' (if !loop then ZMQ.SNDMORE else ZMQ.DONTWAIT)
      | _ -> ZMQ.error()

  /// Operator equivalent to Socket.transfer
  let (>|<) socket target = target |> transfer socket
