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
open System.Runtime.InteropServices

/// <summary>
/// Provides a memory-managed wrapper over ZMQ message operations
/// <remarks>NOTE: For internal use only.</remarks>
/// </summary>
type internal Frame(?source:byte array) =
  let mutable disposed  = false
  let mutable _memory   = Marshal.AllocHGlobal(C.ZMQ_MSG_T_SIZE)

  let (|Source|_|) = function
    | None 
    | Some null -> None
    | Some data -> Some(data |> Array.length |> unativeint,data)

  do (* ctor *) 
    let okay,size,data = 
      match source with
      | Source(size,data) -> C.zmq_msg_init_size(_memory,size),size,data
      | _                 -> C.zmq_msg_init     (_memory     ), 0un,[||]
    if okay <> 0 then ZMQ.error()
    Marshal.Copy(data,0,C.zmq_msg_data(_memory),int size)

  member __.Handle = _memory

  member __.Data =  let size = C.zmq_msg_size(_memory) |> int 
                    let data = C.zmq_msg_data(_memory)
                    let output = Array.zeroCreate<byte> size
                    Marshal.Copy(data,output,0,size) 
                    output
  member __.Size =  C.zmq_msg_size(_memory) |> int
                
  override __.Finalize() = 
    if not disposed then
      disposed <- true
      let okay = C.zmq_msg_close(_memory)
      Marshal.FreeHGlobal(_memory)
      _memory <- 0n
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

  /// <summary>
  /// Pointer to underlying (native) ZMQ socket
  /// <remarks>NOTE: For internal use only.</remarks>
  /// </summary>
  member __.Handle = _socket

  override __.Finalize() =
    if not disposed then
      disposed <- true
      let okay = C.zmq_close(_socket)
      _socket <- 0n
      if okay <> 0 then ZMQ.error()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)


/// Represents the container for a group of sockets in a node</para>
type Context() =

  let mutable disposed = false
  let mutable _context = C.zmq_ctx_new()
  
  do if _context = 0n then ZMQ.error()
  
  /// <summary>
  /// Pointer to underlying (native) ZMQ context
  /// <remarks>NOTE: For internal use only.</remarks>
  /// </summary>
  member __.Handle  = _context

  override __.Finalize() = 
    if not disposed then
      disposed <- true
      let okay = C.zmq_ctx_destroy(_context)
      _context <- 0n
      if okay <> 0 then ZMQ.error()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)
