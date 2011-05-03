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


/// represents any error raised by the native ZMQ library, storage a 
/// human-readable summary of said error in it's 'Message' property
type ZeroMQException private(errnum,errmsg) =
  
  inherit Exception(errmsg)

  new() = 
    let num = C.zmq_errno()
    let msg = C.zmq_strerror(num)
    ZeroMQException(num,msg)

  /// the ZMQ-defined, or OS-defined, error code reported by ZMQ
  member __.ErrorNumber = errnum


/// provides a memory-managed wrapper over ZMQ message operations
type internal Message(?source:byte[]) =
  // MAYBEL consider exposing this class to consumers?
    
  let mutable disposed = false
  let mutable _message = Marshal.AllocHGlobal(C.ZMQ_MSG_T_SIZE)

  do (* ctor *) 
    match source with
    | None 
    | Some(null)  ->  if C.zmq_msg_init(_message) <> 0 
                        then raise <| ZeroMQException()
    | Some(value) ->  let size = value.Length |> unativeint
                      if C.zmq_msg_init_size(_message,size) = 0 
                        then  let data = C.zmq_msg_data(_message)
                              Marshal.Copy(value,0,data,value.Length)
                        else  raise <| ZeroMQException()

  let release () =
    if not disposed then
      disposed <- true
      let okay = C.zmq_msg_close(_message)
      Marshal.FreeHGlobal(_message)
      _message <- 0n
      if okay <> 0 then raise <| ZeroMQException()
  
  member __.Handle  = _message
  member __.Data    = let size = C.zmq_msg_size(_message) |> int
                      let data = C.zmq_msg_data(_message)
                      let output = Array.zeroCreate<byte> size
                      Marshal.Copy(data,output,0,size) 
                      output

  override __.Finalize() = release ()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)


/// an abstraction of an asynchronous message queue, with the exact 
/// queueing semantics depending on the socket type in use
type Socket internal(context,socketType) =

  let mutable disposed  = false
  let mutable _socket   = C.zmq_socket(context,socketType)

  do if _socket = 0n then raise <| ZeroMQException()

  let release () =
    if not disposed then
      disposed <- true
      let okay = C.zmq_close(_socket)
      _socket <- 0n
      if okay <> 0 then raise <| ZeroMQException()

  member internal __.Handle = _socket

  override __.Finalize() = release ()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)

  
/// the container for all sockets in a single process.
/// typically, create and use exactly one context per process. 
type Context(ioThreads) =
  
  let mutable disposed = false
  let mutable _context = C.zmq_init(ioThreads)

  do if _context = 0n then raise <| ZeroMQException()

  let release () =
    if not disposed then
      disposed <- true
      let okay = C.zmq_term(_context)
      _context <- 0n
      if okay <> 0 then raise <| ZeroMQException()

  member internal __.Handle = _context

  override __.Finalize() = release ()

  interface IDisposable with

    member self.Dispose() =
      self.Finalize()
      GC.SuppressFinalize(self)
