(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)
namespace fszmq

open System
open System.Runtime.InteropServices

#nowarn "1182" // value is unused
//NOTE: P/Invoke support in Xamarin Studio is flaky

#nowarn "9" // possible unverifiable IL generation
//NOTE: spurious warning, see `zmq_event_t`,`zmq_pollitem_t` for info

[<RequireQualifiedAccess>]
module internal C =

  type HANDLE     =  nativeint
  type zmq_msg_t  =  nativeint
  type size_t     = unativeint
  type strBuffer  = System.Text.StringBuilder

(* platform *)
  let [<Literal>] SYS_NAMELEN = 256

  [<Struct;StructLayout(LayoutKind.Sequential)>]
  type utsname =
    [<MarshalAs(UnmanagedType.ByValTStr,SizeConst=SYS_NAMELEN)>] val mutable sysname  : string
    [<MarshalAs(UnmanagedType.ByValTStr,SizeConst=SYS_NAMELEN)>] val mutable release  : string
    [<MarshalAs(UnmanagedType.ByValTStr,SizeConst=SYS_NAMELEN)>] val mutable version  : string
    [<MarshalAs(UnmanagedType.ByValTStr,SizeConst=SYS_NAMELEN)>] val mutable machine  : string
    [<MarshalAs(UnmanagedType.ByValTStr,SizeConst=SYS_NAMELEN)>] val mutable nodename : string

  (* :: HACK ::
  Calling this function is used as a hack to determine the current operating
  system. See `Constants.EAGAIN` for more details. *)
  [<DllImport("libc",CallingConvention = CallingConvention.Cdecl)>]
  extern int uname([<In;Out>] utsname& info)

(* version *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern void zmq_version([<Out>] int& major,[<Out>] int& minor,[<Out>] int& patch)

(* error handling *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_errno()

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_strerror(int errno)

(* message *)
  let ZMQ_MSG_T_SIZE =
      let mutable major, minor, patch = 0, 0, 0
      zmq_version(&major, &minor, &patch)
      match major, minor with
      | 4, 0 ->
          48
      | 4, 1 ->
          64
      | _, _ ->
          32

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_init(zmq_msg_t msg)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_msg_init_size(zmq_msg_t msg, size_t size)

  (* :: NOTE ::
  No binding is given for libzmq.zmq_msg_init_data(...), as it is a micro-optimization function.
  For such low-level performance tuning, code should be written in a native (i.e. unmanaged) language. *)

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
  extern int zmq_bind(HANDLE socket, [<MarshalAs(UnmanagedType.LPStr)>] string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_connect(HANDLE socket, [<MarshalAs(UnmanagedType.LPStr)>] string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_unbind(HANDLE socket, [<MarshalAs(UnmanagedType.LPStr)>] string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_disconnect(HANDLE socket, [<MarshalAs(UnmanagedType.LPStr)>] string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_send(HANDLE socket, byte[] buffer, size_t length, int flags)

  (* :: NOTE ::
  No binding is given for libzmq.zmq_send_const(...), as it is a micro-optimization function.
  For such low-level performance tuning, code should be written in a native (i.e. unmanaged) language. *)

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
  extern int zmq_socket_monitor(HANDLE socket, [<MarshalAs(UnmanagedType.LPStr)>] string address, int events)

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
    val mutable fd      : int
    val mutable events  : int16
    val mutable revents : int16

    new(socket,events) = {  socket  = socket
                            fd      = 0
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

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_proxy_steerable (HANDLE frontend, HANDLE backend, HANDLE capture, HANDLE control)

  (* :: NOTE ::
    The zmq_proxy function replaces all previous "in the box" 0MQ devices.
    Said devices API has been deprecated and should no longer be used.
    Additionally, the (optional) capture socket on zmq_proxy is an good
    logging and auditing tool. *)

(* authentication *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_curve_keypair ([<Out>] strBuffer z85_public_key, [<Out>] strBuffer z85_secret_key)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_z85_encode ([<Out>] strBuffer dest, byte[] data, size_t size)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_z85_decode ([<Out>] byte[] dest, [<MarshalAs(UnmanagedType.LPStr)>] string value)
