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
open System.Globalization
open System.Runtime.InteropServices

/// <summary>
/// A version of two possible states:
/// <para>a triple of integers for the major revision, minor revision and 
/// patch number</para>
/// <para>or an `Unknown` indicator</para>
/// </summary>
[<StructuredFormatDisplay("{show}")>]
type Version = Version of int * int * int 
             | Unknown with 

    member private V.show = match V with
                            | Version(m,n,b) -> sprintf "%i.%i.%i" m n b
                            | Unknown        -> "<unknown>"

    override V.ToString() = V.show


/// <summary>
/// <para>Represents any error raised by the native ZMQ library.</para> 
/// <para>Stores a human-readable summary in the `Message` property.</para>
/// </summary> 
type ZMQError internal(errnum,errmsg) =
  inherit Exception(errmsg)

  /// the ZeroMQ-defined, or OS-defined, error code reported by ZMQ
  member __.ErrorNumber = errnum


/// Contains commonly-used pre-defined ZMQ values
[<RequireQualifiedAccess>]
module ZMQ =
  
  /// Version of the underlying (native) ZMQ library
  [<CompiledName("Version")>]
  let version =
    try
      let mutable major,minor,patch = 0,0,0
      C.zmq_version(&major,&minor,&patch)
      match (major,minor,patch) with
      | 0,0,0 -> Unknown
      | m,n,b -> Version(m,n,b)
    with
    | _ -> Unknown


  [<CompiledName("Error")>]
  let internal error() =
    let num = C.zmq_errno()
    let msg = Marshal.PtrToStringAnsi(C.zmq_strerror(num))
    raise <| ZMQError(num,msg)


(* error codes *)
  let [<Literal>] EAGAIN = 11 //TODO: is this cross-platform (WIN + POSIX)?

  let [<Literal>] private HAUSNUMERO = 156384712
  
  let EFSM            = HAUSNUMERO + 51
  let ENOCOMPATPROTO  = HAUSNUMERO + 52
  let ETERM           = HAUSNUMERO + 53
  let EMTHREAD        = HAUSNUMERO + 54

(* context options *)
  let [<Literal>] IO_THREADS  = 1
  let [<Literal>] MAX_SOCKETS = 2
  // default for new contexts
  let [<Literal>] IO_THREADS_DFLT   =    1
  let [<Literal>] MAX_SOCKETS_DFLT  = 1024

(* event codes *)
  let [<Literal>] EVENT_CONNECTED       =   1
  let [<Literal>] EVENT_CONNECT_DELAYED =   2
  let [<Literal>] EVENT_CONNECT_RETRIED =   4
  let [<Literal>] EVENT_LISTENING       =   8
  let [<Literal>] EVENT_BIND_FAILED     =  16
  let [<Literal>] EVENT_ACCEPTED        =  32
  let [<Literal>] EVENT_ACCEPT_FAILED   =  64
  let [<Literal>] EVENT_CLOSED          = 128
  let [<Literal>] EVENT_CLOSE_FAILED    = 256
  let [<Literal>] EVENT_DISCONNECTED    = 512

(* socket types *)
  let [<Literal>] PAIR    =  0
  let [<Literal>] PUB     =  1
  let [<Literal>] SUB     =  2
  let [<Literal>] REQ     =  3
  let [<Literal>] REP     =  4
  let [<Literal>] DEALER  =  5
  let [<Literal>] ROUTER  =  6
  let [<Literal>] PULL    =  7
  let [<Literal>] PUSH    =  8
  let [<Literal>] XPUB    =  9
  let [<Literal>] XSUB    = 10
  // deprecated socket types
  let [<Obsolete;Literal>] XREQ = DEALER
  let [<Obsolete;Literal>] XREP = ROUTER

(* socket options *)

  /// (UInt64) I/O thread affinity bit-mask
  let [<Literal>] AFFINITY                =  4
  /// (Byte[]) Socket identifier
  let [<Literal>] IDENTITY                =  5
  /// (Byte[]) Add subscription filter
  let [<Literal>] SUBSCRIBE               =  6
  /// (Byte[]) Remove subscription filter
  let [<Literal>] UNSUBSCRIBE             =  7
  /// (Int32) Multicast data rate in kilobits per second
  let [<Literal>] RATE                    =  8
  /// (Int32) Multicast recovery period in milliseconds
  let [<Literal>] RECOVERY_IVL            =  9
  /// (Int32) Send-message buffer size in bytes
  let [<Literal>] SNDBUF                  = 11
  /// (Int32) Receive-message buffer size in bytes
  let [<Literal>] RCVBUF                  = 12
  /// (Int32) 1 if more message frames are available, 0 otherwise
  let [<Literal>] RCVMORE                 = 13
  /// (IntPtr) native file descriptor
  let [<Literal>] FD                      = 14
  /// (Int32) Socket event state, see all: Polling
  let [<Literal>] EVENTS                  = 15
  /// (Int32) Socket type
  let [<Literal>] TYPE                    = 16
  /// (Int32) Pause before shutdown in milliseconds
  let [<Literal>] LINGER                  = 17
  /// (Int32) Pause before reconnect in milliseconds
  let [<Literal>] RECONNECT_IVL           = 18
  /// (Int32) Maximum number of queued peers
  let [<Literal>] BACKLOG                 = 19
  /// (Int32) Maximum reconnection interval in milliseconds
  let [<Literal>] RECONNECT_IVL_MAX       = 21
  /// (Int64) Maximum inbound message size in bytes
  let [<Literal>] MAXMSGSIZE              = 22
  /// (Int32) Maximum number of outbound queued messages
  let [<Literal>] SNDHWM                  = 23
  /// (Int32) Maximum number of inbound queued messages
  let [<Literal>] RCVHWM                  = 24
  /// (Int32) Time-to-live for each multicast packet in network-hops
  let [<Literal>] MULTICAST_HOPS          = 25
  /// (Int32) Timeout period for inbound messages in milliseconds
  let [<Literal>] RCVTIMEO                = 27
  /// (Int32) Timeout period for outbound messages in milliseconds
  let [<Literal>] SNDTIMEO                = 28
  /// (Int32) 1 restricts native socket to IPv4 only, 0 also allows IPv6
  let [<Literal>] IPV4ONLY                = 31
  /// (String) Last address bound to endpoint
  let [<Literal>] LAST_ENDPOINT           = 32
  /// (Int32) 1 causes failure on unroutable message, 0 silently ignores
  let [<Literal>] ROUTER_BEHAVIOR         = 33
  /// (Int32) Override OS-level TCP keep-alive
  let [<Literal>] TCP_KEEPALIVE           = 34
  /// (Int32) Override OS-level TCP keep-alive
  let [<Literal>] TCP_KEEPALIVE_CNT       = 35
  /// (Int32) Override OS-level TCP keep-alive
  let [<Literal>] TCP_KEEPALIVE_IDLE      = 36
  /// (Int32) Override OS-level TCP keep-alive
  let [<Literal>] TCP_KEEPALIVE_INTVL     = 37
  /// (Byte[]) TCP/IP filters
  let [<Literal>] TCP_ACCEPT_FILTER       = 38
  /// (Int32) 1 will deplay pipe attachmet until underlying connection completes
  let [<Literal>] DELAY_ATTACH_ON_CONNECT = 39
  /// (Int32) 1 will resend duplicate messages
  let [<Literal>] XPUB_VERBOSE            = 40

(* message options *)
  
  /// (Int32) 1 if more message frames are available, 0 otherwise
  let [<Literal>] MORE = 1

(* transmission options *)
  
  /// Block thread until message frame is sent
  let [<Literal>] WAIT      =   0
  /// Queue message frame for sending (return immediately)
  let [<Literal>] DONTWAIT  =   1
  /// More message frames will follow the current frame
  let [<Literal>] SNDMORE   =   2

(* polling *)
  let [<Literal>] POLLIN  = 1s
  let [<Literal>] POLLOUT = 2s
  let [<Literal>] POLLERR = 4s
  // common timeout lengths for polling
  let [<Literal>] IMMEDIATE =  0L
  let [<Literal>] FOREVER   = -1L
