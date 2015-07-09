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
  let mutable memory    = Marshal.AllocHGlobal(C.ZMQ_MSG_T_SIZE)

  let (|Source|_|) = function
    | None
    | Some null ->  None
    | Some data ->  let size = (Array.length >> unativeint) data
                    Some(size,data)

  do (* ctor *)
    match source with
    | Source(size,data) ->  if C.zmq_msg_init_size(memory,size) <> 0 then ZMQ.error()
                            Marshal.Copy(data,0,C.zmq_msg_data(memory),int size)
    | _                 ->  if C.zmq_msg_init(memory) <> 0 then ZMQ.error()

  /// Creates a new Message from the given byte array
  new (source) = new Message (?source=Some source)

  /// Creates a new empty Message
  new () = new Message (?source=None)

  /// For internal use only
  member __.Handle :nativeint = memory

  override this.GetHashCode () = hash this.Handle

  override this.Equals (obj) =
    match obj with
    | :? Message as that -> this.Handle = that.Handle
    | _                  -> invalidArg "obj" "Argument is not of type Message"

  override this.ToString () = sprintf "Message(%i)" this.Handle 

  override __.Finalize() =
    if not disposed then
      disposed <- true
      let okay = C.zmq_msg_close(memory)
      Marshal.FreeHGlobal(memory)
      memory <- 0n
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
  let mutable _socket   = C.zmq_socket(context,socketType)

  do if _socket = 0n then ZMQ.error()

  /// For internal use only
  member __.Handle :nativeint = _socket

  override this.GetHashCode () = hash this.Handle

  override this.Equals (obj) =
    match obj with
    | :? Socket as that -> this.Handle = that.Handle
    | _                 -> invalidArg "obj" "Argument is not of type Socket"

  override this.ToString () = sprintf "Socket(%i)" this.Handle 

  override __.Finalize() =
    if not disposed then
      disposed <- true
      let okay = C.zmq_close(_socket)
      if okay <> 0 then ZMQ.error()
      _socket  <- 0n

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)


/// Represents the container for a group of sockets in a node
type Context () =
  let mutable disposed = false
  let mutable _context = C.zmq_ctx_new()

  let locker  = obj () // used to synchronize access to `sockets`
  let sockets = ResizeArray<Socket> ()

  do if _context = 0n then ZMQ.error()

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

  member internal __.Attach (socket) =
    lock locker (fun () -> if not <| sockets.Contains socket then sockets.Add socket)

  /// For internal use only
  member __.Handle :nativeint = _context

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
      if C.zmq_ctx_shutdown(_context) = 0
        then  let okay = C.zmq_ctx_term(_context)
              _context <- 0n
              if okay <> 0 then ZMQ.error()
        else  ZMQ.error()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)
