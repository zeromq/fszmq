(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)
namespace fszmq

open System
open System.Globalization
open System.Runtime.InteropServices

/// <summary>
/// A version of two possible states:
/// <para>a triple of integers for the major revision, minor revision, and patch number </para>
/// <para>or an Unknown indicator</para>
/// </summary>
[<StructuredFormatDisplay("{Text}")>]
type Version = Version of major:int * minor:int * revision:int
             | Unknown with

    /// textual representation of Verison
    member private V.Text = match V with
                            | Version(m,n,b) -> sprintf "%i.%i.%i" m n b
                            | Unknown        -> "<unknown>"

    override V.ToString() = V.Text


/// <summary>
/// Represents any error raised by the native ZMQ library,
/// with a human-readable summary in the Message property.
/// </summary>
type ZMQError internal(errnum,errmsg) =
  inherit Exception(errmsg)

  /// the ZeroMQ-defined, or OS-defined, error code
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

  // helper function for build native-to-managed errors
  let inline internal buildError num = ZMQError(num,Marshal.PtrToStringAnsi(C.zmq_strerror(num)))
  // constructs and raises native-to-managed errors
  let inline internal error() = (buildError >> raise) <| C.zmq_errno()
  // helpers for "faking" native errors
  let inline internal einval msg = raise <| ZMQError(22,msg)

(* error codes *)
  let [<Literal>] internal POSIX_EAGAIN = 11
  let [<Literal>] internal BSD_EAGAIN   = 35
  // !!! HACK !!! This whole setup is bad and wrong and should be replaced
  let internal eagain =
    try
      let mutable info = C.utsname()
      C.uname (&info) |> ignore //TODO: handle this better
      match info.sysname.ToLowerInvariant () with
      | "linux"   -> POSIX_EAGAIN  // Linux
      | "darwin"  -> BSD_EAGAIN   // Mac OS X
      //NOTE: this assumes all Unixes are BSD-derived, which is bad and wrong
      | _         -> BSD_EAGAIN
      //TODO: extend this to include other OSes
    with
      | _ -> POSIX_EAGAIN  // Windows
    (* :: NOTE ::
    if _anything_ goes wrong, we assume "libc::uname" doesn't exist (i.e. we're on Windows);
    this is probably bad and wrong and really ought to be replaced with _something_ else.*)

  /// Non-blocking mode was requested and the message cannot be sent at the moment
  let (|EAGAIN|_|) errno =
    if errno = eagain then Some () else None

(* context options *)
  /// (Int32) Set number of OS-level I/O threads
  let [<Literal>] IO_THREADS  = 1
  /// (Int32) Set maximum number of sockets for a context
  let [<Literal>] MAX_SOCKETS = 2

  (* default for new contexts *)
  /// default number of OS-level I/O threads (1)
  let [<Literal>] IO_THREADS_DFLT   =    1
  /// default maximum number of sockets (1024)
  let [<Literal>] MAX_SOCKETS_DFLT  = 1024


(* event codes *)
  let internal EVENT_DETAIL_SIZE = sizeof<uint16> + sizeof<int32>

  /// Socket connection established
  let [<Literal>] EVENT_CONNECTED       =    1us
  /// Synchronous connection failed; socket is being polled
  let [<Literal>] EVENT_CONNECT_DELAYED =    2us
  /// Asynchronous (re)connection attempt
  let [<Literal>] EVENT_CONNECT_RETRIED =    4us
  /// Socket bound to address; ready to accept connections
  let [<Literal>] EVENT_LISTENING       =    8us
  /// Socket could not bind to address
  let [<Literal>] EVENT_BIND_FAILED     =   16us
  /// Connection accepted to bound interface
  let [<Literal>] EVENT_ACCEPTED        =   32us
  /// Could not accept client connection
  let [<Literal>] EVENT_ACCEPT_FAILED   =   64us
  /// Socket connection closed
  let [<Literal>] EVENT_CLOSED          =  128us
  /// Connection could not be closed (only for ipc transport)
  let [<Literal>] EVENT_CLOSE_FAILED    =  256us
  /// Broken session (specific to ipc and tcp transports)
  let [<Literal>] EVENT_DISCONNECTED    =  512us
  /// Event monitoring has been disabled
  let [<Literal>] EVENT_MONITOR_STOPPED = 1024us

  /// Monitor all possible events
  let [<Literal>] EVENT_ALL = EVENT_CONNECTED ||| EVENT_CONNECT_DELAYED ||| EVENT_CONNECT_RETRIED
                          ||| EVENT_LISTENING ||| EVENT_BIND_FAILED
                          ||| EVENT_ACCEPTED  ||| EVENT_ACCEPT_FAILED
                          ||| EVENT_CLOSED    ||| EVENT_CLOSE_FAILED
                          ||| EVENT_DISCONNECTED
                          ||| EVENT_MONITOR_STOPPED


(* socket types *)
  [<Measure>] type SocketType
  
  /// An exclusive pair of two sockets (primarily for use with inproc transport)
  let [<Literal>] PAIR    =  0<SocketType>
  /// A publisher which broadcasts topic-prefixed messages
  let [<Literal>] PUB     =  1<SocketType>
  /// A subscribe which receives topic-prefixed messages
  let [<Literal>] SUB     =  2<SocketType>
  /// Makes synchronous requests of a server (i.e. ZMQ.REP, ZMQ.ROUTER), awaits replies
  let [<Literal>] REQ     =  3<SocketType>
  /// Awaits synchronous requests of a client (i.e. ZMQ.REQ, ZMQ.DEALER), makes replies
  let [<Literal>] REP     =  4<SocketType>
  /// Participates in asynchronous request/reply exchanges with compatible peers (i.e. ZMQ.REP, ZMQ.DEALER, ZMQ.ROUTER)
  let [<Literal>] DEALER  =  5<SocketType>
  /// Participates in asynchronous request/reply exchanges with compatible peers (i.e. ZMQ.REQ, ZMQ.DEALER, ZMQ.ROUTER)
  let [<Literal>] ROUTER  =  6<SocketType>
  /// Collects messages in a fair-queued fashion from across all upstream (i.e. ZMQ.PUSH) nodes
  let [<Literal>] PULL    =  7<SocketType>
  /// Delivers messages in a round-robin fashion to across all downstream (i.e. ZMQ.PULL) nodes
  let [<Literal>] PUSH    =  8<SocketType>
  /// A publisher like ZMQ.PUB, but does not automatically receive forwarded topic subscriptions
  let [<Literal>] XPUB    =  9<SocketType>
  /// A publisher like ZMQ.SUB, but does not automatically forward topic subscriptions
  let [<Literal>] XSUB    = 10<SocketType>
  /// Exchanges raw data with a non-ZeroMQ peer via the tcp transport
  let [<Literal>] STREAM  = 11<SocketType>

  (* deprecated socket types *)

  /// Deprecated. Use ZMQ.DEALER
  let [<Obsolete;Literal>] XREQ = DEALER
  /// Deprecated. Use ZMQ.ROUTER
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
  /// (String) Last address bound to endpoint
  let [<Literal>] LAST_ENDPOINT           = 32
  /// (Int32) 1 to error on unroutable messages, 0 to silently ignore
  let [<Literal>] ROUTER_MANDATORY        = 33
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
  /// (Int32) 1 to limit queuing to only completed connections, 0 otherwise
  let [<Literal>] IMMEDIATE               = 39
  /// (Int32) 1 will resend duplicate messages
  let [<Literal>] XPUB_VERBOSE            = 40
  /// Deprecated. Use ZMQ.STREAM socket instead
  let [<Obsolete;Literal>] ROUTER_RAW     = 41
  /// (Int32) 1 to enable IPv6 on the socket, 0 to restrict to only IPv4
  let [<Literal>] IPV6                    = 42
  /// (Int32) Returns the current security mechanism (0 = NULL, 1 = PLAIN, 2 = CURVE)
  let [<Literal>] MECHANISM               = 43
  /// (Int32) 1 to make socket act as server for PLAIN security, 0 otherwise
  let [<Literal>] PLAIN_SERVER            = 44
  /// (String) Sets the user name for outgoing connections over TCP or IPC
  let [<Literal>] PLAIN_USERNAME          = 45
  /// (String) Sets the password for outgoing connections over TCP or IPC
  let [<Literal>] PLAIN_PASSWORD          = 46
  /// (Int32) 1 to make socket act as server for CURVE security, 0 otherwise
  let [<Literal>] CURVE_SERVER            = 47
  /// (String or Byte[]) sets the long-term public key on a client or server socket
  let [<Literal>] CURVE_PUBLICKEY         = 48
  /// (String or Byte[]) sets the long-term secret key on a client socket
  let [<Literal>] CURVE_SECRETKEY         = 49
  /// (String or Byte[]) sets the long-term server key on a client socket
  let [<Literal>] CURVE_SERVERKEY         = 50
  /// (Int32) 1 to automatically send an empty message on new connection, 0 otherwise
  let [<Literal>] PROBE_ROUTER            = 51
  /// (Int32) 1 to prefix messages with explicit request ID, 0 otherwise
  let [<Literal>] REQ_CORRELATE           = 52
  /// (Int32) 1 to relax strict alternation between ZMQ.REQ and ZMQ.REP, 0 otherwise
  let [<Literal>] REQ_RELAXED             = 53
  /// (Int32) 1 to keep last message in queue (ignores high-water mark options), 0 otherwise
  let [<Literal>] CONFLATE                = 54
  /// (String) Sets authentication domain
  let [<Literal>] ZAP_DOMAIN              = 55

  (* common values *)
  /// (Int32) the value needed to disable lingering on a socket's outbound queue
  let [<Literal>] NO_LINGER = 0

  (* deprecated socket options *)

  /// Deprecated. Do not use.
  let [<Obsolete;Literal>] IPV4ONLY = 31
  /// Deprecated. Use ZMQ.IMMEDAITE
  let [<Obsolete;Literal>] DELAY_ATTACH_ON_CONNECT = IMMEDIATE
  /// Deprecated. Use ZMQ.ROUTER_MANDATORY
  let [<Obsolete;Literal>] FAIL_UNROUTABLE = ROUTER_MANDATORY
  /// Deprecated. Use ZMQ.ROUTER_MANDATORY
  let [<Obsolete;Literal>] ROUTER_BEHAVIOR = ROUTER_MANDATORY


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

  (* deprecated transmission options *)
  /// Deprecated. Use ZMQ.DONTWAIT
  let [<Obsolete;Literal>] NOBLOCK = DONTWAIT


(* polling *)
  /// poll for inbound messages
  let [<Literal>] POLLIN  = 1s
  /// poll for outbound messages
  let [<Literal>] POLLOUT = 2s
  /// poll for messages on stderr (for use with file descriptors)
  let [<Literal>] POLLERR = 4s

  (* common timeout lengths for polling *)
  /// indicates polling should exit immediately
  let [<Literal>] NOW     =  0L
  /// indicates polling should wait indefinitely
  let [<Literal>] FOREVER = -1L


(* proxying *)
  /// Command used to temporarily suspend a steerable proxy
  let PAUSE = "PAUSE"B
  /// Command used to resume a suspended steerable proxy
  let RESUME = "RESUME"B
  /// Command used to cleanly shutdown a steerable proxy
  let TERMINATE = "TERMINATE"B
