(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
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

  /// Sets the given option value for the given Message
  [<Extension;CompiledName("SetOption")>]
  let setOption (message:Message) (messageOption,value) =
    if C.zmq_msg_set(message.Handle,messageOption,value) <> 0 then ZMQ.error()

  /// Returns the content of the given Message
  [<Extension;CompiledName("Data")>]
  let data (message:Message) =
    let size = C.zmq_msg_size(message.Handle) |> int
    let data = C.zmq_msg_data(message.Handle)
    let output = Array.zeroCreate<byte> size
    Marshal.Copy(data,output,0,size)
    output

  /// Returns the size (in bytes) of the given Message
  [<Extension;CompiledName("Size")>]
  let size (message:Message) = C.zmq_msg_size(message.Handle) |> int

  /// Returns true if the given message is a frame in a multi-part message and more frames are available
  [<Extension;CompiledName("HasMore")>]
  let hasMore (message:Message) = C.zmq_msg_more(message.Handle) |> bool

  /// Tests if two `Message` instances have the same size and data
  [<Extension;CompiledName("IsMatch")>]
  let isMatch left right = (size left = size right) && (data left = data right)

(* message manipulation *)

  /// <summary>
  /// Copies the content from one message to another message.
  /// <para> Avoid modifying message content after a message has been copied,
  /// as this can result in undefined behavior.</para>
  /// </summary>
  [<Extension;CompiledName("Copy")>]
  let copy (source:Message) (target:Message) =
    if source.Handle = target.Handle then ZMQ.einval "Invalid argument"
    if C.zmq_msg_copy(target.Handle,source.Handle) <> 0 then ZMQ.error()

  /// <summary>
  /// Moves the content from one message to another message.
  /// <para> No actual copying of message content is performed, target is simply updated to reference the new content.
  /// source becomes an empty message after calling `Message.move()`. The original content of target, if any,
  /// shall be released. To preseve the content of source, see `Message.copy()`.</para>
  /// </summary>
  [<Extension;CompiledName("Move")>]
  let move (source:Message) (target:Message) =
    if source.Handle = target.Handle then ZMQ.einval "Invalid argument"
    if C.zmq_msg_move(target.Handle,source.Handle) <> 0 then ZMQ.error()

  /// Makes a new instance of the Message type, with an independent copy of the source content.
  [<Extension;CompiledName("Clone")>]
  let clone (source:Message) =
    let target = new Message()
    copy source target
    target

(* message sending *)
  let internal (|Okay|Busy|Fail|) = function
    | -1  ->  match C.zmq_errno() with
              | ZMQ.EAGAIN  -> Busy
              | _           -> Fail
    | _   ->  Okay

  let internal waitForOkay fn =
    let rec loop ()  =
      match fn () with
      | true  -> ((* okay *))
      | false -> loop ()
    loop ()

  /// Sends a message, with the given flags, returning true (or false)
  /// if the send was successful (or should be re-tried)
  [<Extension;CompiledName("TrySend")>]
  let trySend (message:Message) (socket:Socket)  flags =
    match C.zmq_msg_send(message.Handle,socket.Handle,flags) with
    | Okay  -> true
    | Busy  -> false
    | Fail  -> ZMQ.error()

  /// Sends a message, indicating no more messages will follow
  [<Extension;CompiledName("Send")>]
  let send message socket  = 
    waitForOkay (fun () -> trySend message socket ZMQ.WAIT)

  /// Sends a message, indicating more messages will follow
  [<Extension;CompiledName("SendMore")>]
  let sendMore message socket  = 
    waitForOkay (fun () -> trySend message socket (ZMQ.WAIT ||| ZMQ.SNDMORE))

  /// Operator equivalent to `Message.send`
  let (<<-) socket message = send message socket
  /// Operator equivalent to `Message.sendMore`
  let (<<+) socket message = sendMore socket message

  /// Operator equivalent to `Message.send` (with arguments reversed)
  let (->>) message socket = socket <<- message
  /// Operator equivalent to `Message.sendMore` (with arguments reversed)
  let (+>>) message socket = socket <<+ message

(* message receiving *)

  /// Updates the given `Message` instance with the next available message from a socket, 
  /// returning true (or false) if the recv was successful (or should be re-tried)
  [<Extension;CompiledName("TryRecv")>]
  let tryRecv (message:Message) (socket:Socket) flags =
    match C.zmq_msg_recv(message.Handle,socket.Handle,flags) with
    | Okay -> true
    | Busy -> false
    | Fail -> ZMQ.error()

  /// Updates the given `Message` instance with the next available message from a socket;
  /// If no message is received before RCVTIMEO expires, throws a TimeoutException
  [<Extension;CompiledName("Recv")>]
  let recv message socket =
    if not <| tryRecv message socket ZMQ.WAIT then raise <| TimeoutException ()

  /// Operator equivalent to `Message.recv`
  let (|<<) message socket = recv message socket

  /// Operator equivalent to `Message.recv` (with arguments reversed)
  let (>>|) socket message = message |<< socket
