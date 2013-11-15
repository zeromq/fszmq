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

/// Contains methods for working with Message instances
[<Extension;CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Message =

(* message options *)

  /// Gets the value of the given option for the given Message
  [<Extension;CompiledName("GetOption")>]
  let getOption (message:Message) messageOption = 
    match C.zmq_msg_get(message.Handle,messageOption) with
    |    -1 -> ZMQ.error()
    | value -> value

  /// Sets the given option value for the given Socket
  [<Extension;CompiledName("SetOption")>]
  let setOption (message:Message) (messageOption,value) = 
    if C.zmq_msg_set(message.Handle,messageOption,value) <> 0 then ZMQ.error()

  /// Returns the content of the given Message
  [<Extension;CompiledName("GetData")>]
  let data (message:Message) =
    let size = C.zmq_msg_size(message.Handle) |> int 
    let data = C.zmq_msg_data(message.Handle)
    let output = Array.zeroCreate<byte> size
    Marshal.Copy(data,output,0,size) 
    output
  
  /// Returns the size (in bytes) of the given Message
  [<Extension;CompiledName("GetSize")>]
  let size (message:Message) = C.zmq_msg_size(message.Handle) |> int
    
  /// Returns true if the given message is a frame in a multi-part message and more frames are available
  [<Extension;CompiledName("HasMore")>]
  let more (message:Message) = C.zmq_msg_more(message.Handle) |> bool

(* message sending *)
  let private (|Okay|Busy|Fail|) = function 
    | -1  ->  match C.zmq_errno() with 
              | ZMQ.EAGAIN  -> Busy
              | _           -> Fail
    | _   ->  Okay

  /// Sends a frame, with the given flags, returning true (or false) 
  /// if the send was successful (or should be re-tried)
  [<Extension;CompiledName("TrySend")>]
  let trySend (message:Message) (socket:Socket) flags =
    match C.zmq_msg_send(message.Handle,socket.Handle,flags) with
    | Okay  -> true
    | Busy  -> false
    | Fail  -> ZMQ.error()

  let private waitToSend message socket flags =
    let rec send' ()  =
      match trySend message socket flags with
      | true  -> ((* okay *))
      | false -> send'()
    send'()

  /// Sends a frame, indicating no more frames will follow
  [<Extension;CompiledName("Send")>]
  let send message socket = waitToSend message socket ZMQ.WAIT

  /// Sends a frame, indicating more frames will follow, 
  [<Extension;CompiledName("SendMore")>]
  let sendMore message socket = waitToSend message socket ZMQ.SNDMORE

(* message receiving *)

  /// Gets the next available frame from a socket, returning a Message option 
  /// where None indicates the operation should be re-attempted
  [<Extension;CompiledName("TryRecv")>]
  let tryRecv (socket:Socket) flags =
    let frame = new Message()
    match C.zmq_msg_recv(frame.Handle,socket.Handle,flags) with
    | Okay -> Some(frame)
    | Busy -> None
    | Fail -> ZMQ.error()

  /// Waits for (and returns) the next available Message from a socket
  [<Extension;CompiledName("Recv")>]
  let recv socket = Option.get (tryRecv socket ZMQ.WAIT)
