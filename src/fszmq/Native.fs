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

  type zmq_free_fn = delegate of data:HANDLE * hint:HANDLE -> unit

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_init(zmq_msg_t msg)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_init_size(zmq_msg_t msg, size_t size)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_init_data(zmq_msg_t msg, HANDLE data, size_t size, zmq_free_fn ffn, HANDLE hint)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_move(zmq_msg_t target, zmq_msg_t source)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_copy(zmq_msg_t target, zmq_msg_t source)

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
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_send(HANDLE socket, byte[] buffer, size_t length, int flags)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_send_const(HANDLE socket, HANDLE buffer, size_t length, int flags)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_recv(HANDLE socket,[<Out>] byte[] buffer, size_t length, int flags)

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
