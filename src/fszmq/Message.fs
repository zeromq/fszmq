(* ------------------------------------------------------------------------
This file is part of fszmq.

fszmq is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published 
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

fszmq is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with fszmq. If not, see <http://www.gnu.org/licenses/>.

Copyright (c) 2011-2013 Paulmichael Blasucci
------------------------------------------------------------------------ *)
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

(* message manipulation *)

  /// <summary>
  /// Copies the content from one message to another message.
  /// <para>Avoid modifying message content after a message has been copied, as this can result in undefined behavior. 
  /// If what you need is an actual hard copy, see `Message.clone()`</para>
  /// </summary>
  [<Experimental("The function may lead to unstable code or may be removed in a future version. Use with caution.")>]
  [<Extension;CompiledName("CopyTo")>]
  let copy (source:Message) (target:Message) =
    if C.zmq_msg_copy(target.Handle,source.Handle) <> 0 then ZMQ.error()
  
  /// <summary>
  /// Moves the content from one message to another message.
  /// <para>No actual copying of message content is performed, `target` is simply updated to reference the new content. 
  /// `source` becomes an empty message after calling `Message.move()`. 
  /// The original content of `target`, if any, shall be released.</para>
  /// </summary>
  [<Experimental("The function may lead to unstable code or may be removed in a future version. Use with caution.")>]
  [<Extension;CompiledName("MoveTo")>]
  let move (source:Message) (target:Message) =
    if C.zmq_msg_move(target.Handle,source.Handle) <> 0 then ZMQ.error()

  /// Makes a new instance of the `Message` type, with an independent copy of the `source` content.
  [<Extension;CompiledName("Clone")>]
  let clone (source:Message) = 
    let target = new Message()
    Marshal.Copy(data source,0,C.zmq_msg_data(target.Handle),size source)
    target
      
(* message sending *)
  let internal (|Okay|Busy|Fail|) = function 
    | -1  ->  match C.zmq_errno() with 
              | ZMQ.EAGAIN  -> Busy
              | _           -> Fail
    | _   ->  Okay

  let internal waitForOkay fn socket flags =
    let rec send' ()  =
      match fn socket flags with
      | true  -> ((* okay *))
      | false -> send'()
    send'()

  /// Sends a frame, with the given flags, returning true (or false) 
  /// if the send was successful (or should be re-tried)
  [<Extension;CompiledName("TrySend")>]
  let trySend (message:Message) (socket:Socket) flags =
    match C.zmq_msg_send(message.Handle,socket.Handle,flags) with
    | Okay  -> true
    | Busy  -> false
    | Fail  -> ZMQ.error()

  /// Sends a frame, indicating no more frames will follow
  [<Extension;CompiledName("Send")>]
  let send socket message = waitForOkay (trySend message) socket ZMQ.WAIT

  /// Sends a frame, indicating more frames will follow, 
  [<Extension;CompiledName("SendMore")>]
  let sendMore socket message = waitForOkay (trySend message) socket (ZMQ.WAIT ||| ZMQ.SNDMORE)

  /// Operator equivalent to Message.send
  let (<<-) socket = send socket
  /// Operator equivalent to Message.sendMore
  let (<<+) socket = sendMore socket

  /// Operator equivalent to Message.send (with arguments reversed)
  let (->>) message socket = socket <<- message
  /// Operator equivalent to Message.sendMore (with arguments reversed)
  let (+>>) message socket = socket <<+ message

(* message receiving *)

  /// Gets the next available frame from a socket, returning a Message option 
  /// where None indicates the operation should be re-attempted
  [<CompiledName("TryRecv")>]
  let tryRecv (socket:Socket) flags =
    let frame = new Message()
    match C.zmq_msg_recv(frame.Handle,socket.Handle,flags) with
    | Okay -> Some(frame)
    | Busy -> None
    | Fail -> ZMQ.error()

  /// Waits for (and returns) the next available Message from a socket
  [<CompiledName("Recv")>]
  let recv socket = Option.get (tryRecv socket ZMQ.WAIT)
