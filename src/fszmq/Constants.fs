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

#nowarn "51" // use of native pointers may result in unverifiable IL code

open System

/// <summary>
/// A version of two possible states:
/// <para>a triple of integers for the major revision, minor 
/// revision and patch number</para>
/// <para>or an "Unknown" indicator</para>
/// </summary>
[<StructuredFormatDisplay("v{show}")>]
type Version = Version of int * int * int 
             | Unknown with 
  member private self.show = match self with
                             | Version(m,n,b) -> sprintf "%i.%i.%i" m n b
                             | Unknown        -> "<unknown>"
  override self.ToString() = self.show


/// Contains commonly-used pre-defined ZeroMQ values
[<RequireQualifiedAccess>]
module ZMQ =
  
  /// Version of the native ZeroMQ library
  [<CompiledName("Version")>]
  let version =
    try
      let mutable major,minor,patch = 0,0,0
      C.zmq_version(&&major,&&minor,&&patch)
      match (major,minor,patch) with
      | 0,0,0 -> Unknown
      | m,n,b -> Version(m,n,b)
    with
    | _ -> Unknown

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
  // deprecated... will be removed in 3.x
  let [<Obsolete;Literal>] XREQ        = DEALER
  let [<Obsolete;Literal>] XREP        = ROUTER
  let [<Obsolete;Literal>] UPSTREAM    = PULL
  let [<Obsolete;Literal>] DOWNSTREAM  = PUSH

(* socket options *)
  
  /// (UInt64) "High-water mark" message count
  let [<Literal>] HWM               =  1
  /// (Int64) Swap-disk bytes
  let [<Obsolete;Literal>] SWAP     =  3 // NOTE: will be dropped in v3.x
  /// (UInt64) I/O thread affinity bit-mask
  let [<Literal>] AFFINITY          =  4
  /// (Byte[]) Identity
  let [<Literal>] IDENTITY          =  5
  /// (Byte[]) Add subscription filter
  let [<Literal>] SUBSCRIBE         =  6
  /// (Byte[]) Drop subscription filter
  let [<Literal>] UNSUBSCRIBE       =  7
  /// (Int64) Multicast data rate seconds
  let [<Literal>] RATE              =  8
  /// (Int64) Multicast recovery seconds 
  let [<Literal>] RECOVERY_IVL      =  9
  /// (Int64) Multicast loop-back (yes or no)
  let [<Literal>] MCAST_LOOP        = 10
  /// (UInt64) Transmit buffer bytes
  let [<Literal>] SNDBUF            = 11
  /// (UInt64) Receive buffer bytes
  let [<Literal>] RCVBUF            = 12
  /// (Int32) Shutdown linger milliseconds
  let [<Literal>] LINGER            = 17
  /// (Int32) Reconnect pause milliseconds
  let [<Literal>] RECONNECT_IVL     = 18
  /// (Int32) Maximum outstanding queued peers
  let [<Literal>] BACKLOG           = 19
  /// (Int64) Multicast recovery milliseconds
  let [<Literal>] RECOVERY_IVL_MSEC = 20
  /// (Int32) Total reconnect pause milliseconds
  let [<Literal>] RECONNECT_IVL_MAX = 21
  /// (NativeInt) File descriptor associated with socket
  let [<Literal>] FD                = 14
  /// (UInt32) Socket event state, see also: `Polling`
  let [<Literal>] EVENTS            = 15
  /// (Int32) Socket type
  let [<Literal>] TYPE              = 16

(* transmission options *)

  let [<Literal>] BLOCK   =  0
  let [<Literal>] NOBLOCK =  1
  let [<Literal>] SNDMORE =  2
  let [<Literal>] RCVMORE = 13

(* polling *)

  // events for `PollItem` instances
  let [<Literal>] POLLIN  = 1s
  let [<Literal>] POLLOUT = 2s
  let [<Literal>] POLLERR = 4s
  // common timeout lengths for `Polling.poll`
  let [<Literal>] IMMEDIATE =  0L
  let [<Literal>] FOREVER   = -1L
