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
open System.Runtime.InteropServices

/// Provides a memory-managed wrapper over ZMQ message operations
type Message(?source:byte array) =
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

  /// Pointer to underlying (native) ZMQ message
  /// 
  /// ** Note: For internal use only. **
  member __.Handle :nativeint = memory

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

  let cancelLinger () =
    useBuffer (fun (size,buffer) -> writeInt32 ZMQ.NO_LINGER buffer
                                    let okay = C.zmq_setsockopt(_socket,ZMQ.LINGER,buffer,unativeint size)
                                    if  okay <> 0 then ZMQ.error()) 
              sizeof<Int32>

  do if _socket = 0n then ZMQ.error()

  /// Pointer to underlying (native) ZMQ socket
  /// 
  /// ** Note: For internal use only. **
  member __.Handle :nativeint = _socket

  override __.Finalize() =
    if not disposed then
      disposed <- true
      cancelLinger ()
      let okay = C.zmq_close(_socket)
      if okay <> 0 then ZMQ.error()
      _socket  <- 0n
      
  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)


/// Represents the container for a group of sockets in a node
type Context() =

  let mutable disposed = false
  let mutable _context = C.zmq_ctx_new()
  
  do if _context = 0n then ZMQ.error()
  
  /// Pointer to underlying (native) ZMQ context
  /// 
  /// ** Note: For internal use only. **
  member __.Handle :nativeint = _context

  override __.Finalize() = 
    if not disposed then
      disposed <- true
      if C.zmq_ctx_shutdown(_context) = 0
        then  let okay = C.zmq_ctx_term(_context)
              _context <- 0n
              if okay <> 0 then ZMQ.error()
        else  ZMQ.error()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)
