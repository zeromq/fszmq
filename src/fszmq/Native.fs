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

#nowarn "9" // possible unverifiable IL generation
//NOTE: spurious warning, see `zmq_event_t`,`zmq_pollitem_t` for info

[<RequireQualifiedAccess>]
module internal C =

  type HANDLE     =  nativeint
  type zmq_msg_t  =  nativeint
  type size_t     = unativeint
  type strBuffer  = System.Text.StringBuilder
  
(* version *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern void zmq_version([<Out>] int& major,[<Out>] int& minor,[<Out>] int& patch)

(* error handling *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_errno()

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_strerror(int errno)

(* message *)
  let [<Literal>] ZMQ_MSG_T_SIZE = 32

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_init(zmq_msg_t msg)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_init_size(zmq_msg_t msg, size_t size)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_send(zmq_msg_t msg, HANDLE socket, int flags)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_recv(zmq_msg_t msg, HANDLE socket, int flags)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_close(zmq_msg_t msg)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern void *zmq_msg_data(zmq_msg_t msg)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern size_t zmq_msg_size(zmq_msg_t msg)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_more(zmq_msg_t msg)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_get(zmq_msg_t msg, int option)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_set(zmq_msg_t msg, int option, int optval)

  (* :: MAYBE ::
    Does it make sense to implement the functions 
      `zmq_msg_init_data`
      `zmq_msg_move`
      `zmq_msg_copy` 
    in a memory-managed environment? 
    If you find yourself needing the nano-second speed-ups those methods offer, 
    shouldn't you be working directly in the native library? *)

(* socket *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_socket(HANDLE context, int socketType)
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_close(HANDLE socket)
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_setsockopt(HANDLE socket, int option, HANDLE value, size_t size)
   
   [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_getsockopt(HANDLE socket, int option, HANDLE value, [<Out>] size_t& size)
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_bind(HANDLE socket, [<MarshalAs(UnmanagedType.AnsiBStr)>] string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_connect(HANDLE socket, [<MarshalAs(UnmanagedType.AnsiBStr)>] string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_unbind(HANDLE socket, [<MarshalAs(UnmanagedType.AnsiBStr)>] string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_disconnect(HANDLE socket, [<MarshalAs(UnmanagedType.AnsiBStr)>] string address)
  
  (* :: MAYBE ::
    Does it make sense to implement the functions 
      `zmq_send`
      `zmq_recv`
      `zmq_send_const` 
    in a memory-managed environment? 
    These functions have overlap with several `zmq_msg_*` functions,
    and might not be feasible without direct buffer access. *)

  [<Struct;StructLayout(LayoutKind.Sequential)>]
  type zmq_event_t =
    val mutable event : uint16  // ID of the event as bit-field
    val mutable value : int     // value is either error code, FD, or reconnect interval
    
    (* :: NOTE :: 
    With the current F# compiler, any use of the StructLayout attribute 
    produces the warning: "Uses of this construct may result in the 
    generation of unverifiable .NET IL code". However, zmq_event_t
    does _not_ have "explicit packing and an object reference which  
    overlaps a built-in value type or a part of another object 
    reference". So, at least by the ECMA-355 specification, it should
    still produce verifiable code. A quick pass of fszmq.dll through 
    peverify.exe confirms this safety assertion. Microsoft has 
    acknowledged this bug, but considers it a low priority. Thus, a 
    #nowarn pragma has been included near the top of the this file. *)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_socket_monitor(HANDLE socket, [<MarshalAs(UnmanagedType.AnsiBStr)>] string address, int events)
    
(* context *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_ctx_new()

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_term(HANDLE context)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_shutdown(HANDLE context)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_set(HANDLE context, int option, int value)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_get(HANDLE context, int option)
 
(* polling *)
  [<Struct;StructLayout(LayoutKind.Sequential)>]
  type zmq_pollitem_t =
    val mutable socket  : HANDLE  // if socket _and_ fd are set, socket takes precedence
    val mutable fd      : HANDLE
    val mutable events  : int16
    val mutable revents : int16

    new(socket,events) = {  socket  = socket
                            fd      = 0n 
                            events  = events
                            revents = 0s }
  (* :: NOTE :: 
    With the current F# compiler, any use of the StructLayout attribute 
    produces the warning: "Uses of this construct may result in the 
    generation of unverifiable .NET IL code". However, zmq_pollitem_t
    does _not_ have "explicit packing and an object reference which  
    overlaps a built-in value type or a part of another object 
    reference". So, at least by the ECMA-355 specification, it should
    still produce verifiable code. A quick pass of fszmq.dll through 
    peverify.exe confirms this safety assertion. Microsoft has 
    acknowledged this bug, but considers it a low priority. Thus, a 
    #nowarn pragma has been included near the top of the this file. *)
    
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_poll ([<In;Out>] zmq_pollitem_t[] items, int count, int64 timeout)

(* proxy *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_proxy (HANDLE frontend, HANDLE backend, HANDLE capture)

  (* :: NOTE ::
    The zmq_proxy function replaces all previous "in the box" 0MQ devices. 
    Said devices API has been deprecated and should no longer be used.     
    Additionally, the (optional) capture socket on zmq_proxy is an good    
    logging and auditing tool. *)

(* authentication *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_curve_keypair ([<Out>] strBuffer z85_public_key, [<Out>] strBuffer z85_secret_key)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern [<return: MarshalAs(UnmanagedType.AnsiBStr)>] 
         string zmq_z85_encode (strBuffer dest, [<Out>] byte[] data, size_t size)
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_z85_decode ([<In;Out>] byte[] dest, [<MarshalAs(UnmanagedType.AnsiBStr)>] string value)
