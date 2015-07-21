(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)
namespace fszmq

open System
open System.Runtime.InteropServices

/// Provides a memory-managed wrapper over ZMQ message operations
type Message private(?source:byte array) =
  let mutable disposed  = false
  let mutable handle    = Marshal.AllocHGlobal(ZMQ.ZMQ_MSG_T_SIZE)

  let (|Source|_|) = function
    | None
    | Some null ->  None
    | Some data ->  let size = (Array.length >> unativeint) data
                    Some(size,data)

  do (* ctor *)
    match source with
    | Source(size,data) ->  if C.zmq_msg_init_size(handle,size) <> 0 then ZMQ.error()
                            Marshal.Copy(data,0,C.zmq_msg_data(handle),int size)
    | _                 ->  if C.zmq_msg_init(handle) <> 0 then ZMQ.error()

  /// Creates a new Message from the given byte array
  new (source) = new Message (?source=Some source)

  /// Creates a new empty Message
  new () = new Message (?source=None)

  /// For internal use only
  member __.Handle :nativeint = handle

  override this.GetHashCode () = hash this.Handle

  override this.Equals (obj) =
    match obj with
    | :? Message as that -> this.Handle = that.Handle
    | _                  -> invalidArg "obj" "Argument is not of type Message"

  override this.ToString () = sprintf "Message(%i)" this.Handle 

  override __.Finalize() =
    if not disposed then
      disposed <- true
      let okay = C.zmq_msg_close(handle)
      Marshal.FreeHGlobal(handle)
      handle <- 0n
      if okay <> 0 then ZMQ.error()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)


/// An abstraction of an asynchronous message queue,
/// with the exact queuing and message-exchange
/// semantics determined by the socket type
type Socket internal(context,socketType) =
  let mutable disposed  = false
  let mutable handle    = C.zmq_socket(context,socketType)

  do if handle = 0n then ZMQ.error()

  /// For internal use only
  member __.Handle :nativeint = handle

  override this.GetHashCode () = hash this.Handle

  override this.Equals (obj) =
    match obj with
    | :? Socket as that -> this.Handle = that.Handle
    | _                 -> invalidArg "obj" "Argument is not of type Socket"

  override this.ToString () = sprintf "Socket(%i)" this.Handle 

  override __.Finalize() =
    if not disposed then
      disposed <- true
      let okay = C.zmq_close(handle)
      if okay <> 0 then ZMQ.error()
      handle  <- 0n

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)


/// Represents the container for a group of sockets in a node
type Context private (__) = 
  (* ^^^ HACK: used to work around an XMLDoc bug ^^^ *)
  let mutable disposed  = false
  let mutable handle    = C.zmq_ctx_new()

  let locker  = obj () // used to synchronize access to `sockets`
  let sockets = ResizeArray<Socket> ()

  do if handle = 0n then ZMQ.error()

  let closeSockets () =
    useBuffer sizeof<Int32> (fun (size,buffer) ->
      writeInt32 ZMQ.NO_LINGER buffer
      while sockets.Count > 0 do
        let socket = sockets.Item 0
        sockets.RemoveAt 0
        try
          C.zmq_setsockopt(socket.Handle,ZMQ.LINGER,buffer,size) |> ignore
        finally
          (socket :> IDisposable).Dispose ())
  
  /// Initializes a new Context instance
  new () = new Context (null)

  member internal __.Attach (socket) =
    lock locker (fun () -> if not <| sockets.Contains socket then sockets.Add socket)

  /// For internal use only
  member __.Handle :nativeint = handle

  override this.GetHashCode () = hash this.Handle

  override this.Equals (obj) =
    match obj with
    | :? Context as that -> this.Handle = that.Handle
    | _                  -> invalidArg "obj" "Argument is not of type Context"

  override this.ToString () = sprintf "Context(%i)" this.Handle 

  override __.Finalize() =
    if not disposed then
      disposed <- true
      lock locker (fun () -> closeSockets ())
      if C.zmq_ctx_shutdown(handle) = 0
        then  let okay = C.zmq_ctx_term(handle)
              handle <- 0n
              if okay <> 0 then ZMQ.error()
        else  ZMQ.error()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)
