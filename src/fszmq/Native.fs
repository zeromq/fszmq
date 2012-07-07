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

#nowarn "9" // possible unverifiable IL generation

[<RequireQualifiedAccess>]
module internal C =

  open System
  open System.Runtime.InteropServices

  type HANDLE =  nativeint
  type size_t = unativeint

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern void zmq_version (int& major, int& minor, int& patch)

(* error handling *)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_errno ()
  
  [<DllImport("libzmq",
              CharSet = CharSet.Ansi,
              CallingConvention=CallingConvention.Cdecl)>]
  extern string zmq_strerror (int errnum)
  
(* message *)

  let [<Literal>] ZMQ_MAX_VSM_SIZE = 30
  let ZMQ_MSG_T_SIZE = sizeof<nativeint> 
                     + sizeof<byte> // message flags 
                     + sizeof<byte> // message size
                     + ZMQ_MAX_VSM_SIZE

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_msg_init (HANDLE msg)
  
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_msg_init_size (HANDLE msg, size_t size)
  
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern size_t zmq_msg_size (HANDLE msg)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern HANDLE zmq_msg_data (HANDLE msg)
  
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_msg_close (HANDLE msg)

(* socket *)
  
  [<Literal>]
  let EAGAIN = 11 // WARN: this only works on recent Microsoft OSes
                  // TODO: find the POSIX version for conditional compile

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern HANDLE zmq_socket (HANDLE context, int socket_type)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_setsockopt (HANDLE socket, 
                             int option, 
                             HANDLE value, 
                             size_t size)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_getsockopt (HANDLE socket, 
                             int option, 
                             HANDLE value, 
                             size_t& size)
  
  [<DllImport("libzmq",
              CharSet = CharSet.Ansi,
              CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_bind (HANDLE socket, string address)

  [<DllImport("libzmq",
              CharSet = CharSet.Ansi,
              CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_connect (HANDLE socket, string address)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_send (HANDLE socket, HANDLE msg, int flags)
  
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_recv (HANDLE socket, HANDLE msg, int flags)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_close (HANDLE socket)

(* context *)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern HANDLE zmq_init (int io_threads)

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_term (HANDLE context)

(* polling *)

  [<Struct>]
  [<StructLayout(LayoutKind.Sequential)>]
  type zmq_pollitem_t =
    struct
      val mutable socket  : HANDLE
      val mutable fd      : HANDLE
      val mutable events  : int16
      val mutable revents : int16

      new(socket,events) = {  socket  = socket
                              fd      = 0n 
                              events  = events
                              revents = 0s }

    //static member ofFD(fd,events) = zmq_pollitem_t(0n,events,fd=fd)
    end

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern int zmq_poll ([<In;Out>]zmq_pollitem_t[] items, 
                                 int              count, 
                                 int64            timeout)
