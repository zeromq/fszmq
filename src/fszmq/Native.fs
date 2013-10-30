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
//NOTE: spurious warning, see `zmq_event_data_t`,`zmq_pollitem_t` for info

[<RequireQualifiedAccess>]
module internal C =

  type HANDLE     =  nativeint
  type zmq_msg_t  =  nativeint
  type size_t     = unativeint
  
(* version *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern void zmq_version(int& major, int& minor, int& patch)

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

  (* :: NOTE ::
    The functions `zmq_msg_init_data`, `zmq_msg_move`, and `zmq_msg_copy` 
    have NOT been implemented because they make no real sense in a 
    memory-managed environment. If you find yourself needing the 
    nano-second speed-ups those methods offer, you should be working 
    directly in the native library -- and NOT in a higher-level binding *)

(* socket *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_socket(HANDLE context, int socketType)
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_close(HANDLE socket)
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_setsockopt(HANDLE  socket
                           ,int     option
                           ,HANDLE  value
                           ,size_t  size)
   
   [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_getsockopt(HANDLE  socket
                           ,int     option
                           ,HANDLE  value
                           ,size_t& size)
  
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_bind(HANDLE socket,
                      [<MarshalAs(UnmanagedType.AnsiBStr)>] 
                      string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_connect(HANDLE socket,
                         [<MarshalAs(UnmanagedType.AnsiBStr)>] 
                         string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_unbind(HANDLE socket,
                        [<MarshalAs(UnmanagedType.AnsiBStr)>] 
                        string address)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_disconnect(HANDLE socket,
                            [<MarshalAs(UnmanagedType.AnsiBStr)>] 
                            string address)

(* context *)
  [<StructLayout(LayoutKind.Sequential
                ,CharSet = CharSet.Ansi)>]
  type zmq_event_data_t =
    struct
      val mutable address : string
      val mutable details : int

      (* :: NOTE :: 
      With the current F# compiler, any use of the StructLayout attribute 
      produces the warning: "Uses of this construct may result in the 
      generation of unverifiable .NET IL code". However, zmq_event_data_t
      does _not_ have "explicit packing and an object reference which  
      overlaps a built-in value type or a part of another object 
      reference". So, at least by the ECMA-355 specification, it should
      still produce verifiable code. A quick pass of fszmq.dll through 
      peverify.exe confirms this saftey assertion. Microsoft has 
      acknowledged this bug, but considers it a low priority. Thus, a 
      #nowarn pragma has been included near the top of the this file. *)
    end

  type zmq_monitor_fn = delegate of HANDLE * int * zmq_event_data_t -> unit

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern HANDLE zmq_ctx_new()

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_destroy(HANDLE context)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_set(HANDLE context, int option, int value)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_get(HANDLE context, int option)

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_ctx_set_monitor (HANDLE context, zmq_monitor_fn monitor)
  
(* polling *)
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

    (* :: NOTE :: 
      With the current F# compiler, any use of the StructLayout attribute 
      produces the warning: "Uses of this construct may result in the 
      generation of unverifiable .NET IL code". However, zmq_pollitem_t
      does _not_ have "explicit packing and an object reference which  
      overlaps a built-in value type or a part of another object 
      reference". So, at least by the ECMA-355 specification, it should
      still produce verifiable code. A quick pass of fszmq.dll through 
      peverify.exe confirms this saftey assertion. Microsoft has 
      acknowledged this bug, but considers it a low priority. Thus, a 
      #nowarn pragma has been included near the top of the this file. *)
    end

  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_poll ([<In;Out>]zmq_pollitem_t[] items, 
                                 int              count, 
                                 int64            timeout)

(* proxy *)
  [<DllImport("libzmq",CallingConvention = CallingConvention.Cdecl)>]
  extern int zmq_proxy (HANDLE frontend, HANDLE backend, HANDLE capture)

  (* :: NOTE ::
    The zmq_proxy function replaces all previous "in the box" 0MQ devices. 
    Said devices API has been deprecated and should no longer be used.     
    Additionally, the (optional) capture socket on zmq_proxy is an good    
    logging and auditing tool. *)
