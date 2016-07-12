(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)
namespace fszmq

open System

/// Higher-level API for working with socket options
module Options =
  /// binary unit of information
  type [<Measure>] bit
  /// abbreviation for `bit`
  type [<Measure>] b = bit
  /// 100 bits
  type [<Measure>] kilobit
  /// abbreviation for `kilobit`
  type [<Measure>] kb = kilobit
  /// standard unit of time (1/60th of a minute)
  type [<Measure>] second = FSharp.Data.UnitSystems.SI.UnitNames.second
  /// abbreviation for `second`
  type [<Measure>] s = second
  /// 1/1000th of a second
  type [<Measure>] millisecond
  /// abbreviation for `milliecond`
  type [<Measure>] ms = millisecond
  /// 8 bits
  type [<Measure>] Byte
  /// abbreviation for `Byte`
  type [<Measure>] B = Byte
  /// unit of travel from one network switch to another
  type [<Measure>] NetworkHop
  /// abbreviation for `NetworkHop`
  type [<Measure>] hop = NetworkHop
  
  /// Settable socket options
  type SocketOption =
    /// I/O thread affinity bit-mask
    | Affinity of bitmask:uint64
    /// Socket identifier
    | Identity of identifier:byte[]
    /// Add subscription filter
    | Subscribe of filter:byte[]
    /// Remove subscription filter
    | Unsubscribe of filter:byte[]
    /// Multicast data rate
    | Rate of dataRate:int<kb/s>
    /// Multicast recovery period
    | MulticastRecovery of time:int<ms>
    /// Send-message buffer size in bytes
    | SendBuffer of size:int<Byte>
    /// Receive-message buffer size in bytes
    | ReceiveBuffer of int<Byte>
    /// Pause before shutdown
    | LingerDelay of delay:int<ms>
    /// Pause before reconnect
    | ReconnectDelay of delay:int<ms>
    /// Maximum number of queued peers
    | Backlog of length:int
    /// Maximum reconnection interval
    | MaxReconnectInterval of delay:int<ms>
    /// Maximum inbound message size
    | MaxMessageSize of size:int64<Byte>
    /// Maximum number of outbound queued messages
    | SendQueue of size:int
    /// Maximum number of inbound queued messages
    | ReceiveQueue of size:int
    /// Time-to-live for each multicast packet
    | MulticastHops of hops:int<NetworkHop>
    /// Timeout period for inbound messages
    | ReceiveTimeout of timeout:int<ms> 
    /// Timeout period for outbound messages
    | SendTimeout of timeout:int<ms>
    /// true to error on unroutable messages, false to silently ignore
    | RouterMandatory of errorOnUnroutable:bool
    /// Override OS-level TCP keep-alive
    | TcpKeepalive of enable:bool option
    /// Override OS-level TCP keep-alive
    | TcpKeepaliveCount of count:int
    /// Override OS-level TCP keep-alive
    | TcpKeepaliveIdle of idle:int<s>
    /// Override OS-level TCP keep-alive
    | TcpKeepaliveInterval of delay:int<s>
    /// limit queuing to only completed connections
    | Immediate of queueCompletedOnly:bool
    /// resend duplicate messages
    | ResendDuplicateMessages of resendDuplicate:bool
    /// true to enable IPv6 on the socket, false to restrict to only IPv4
    | Ipv6 of ipv6:bool
    /// automatically send an empty message on new connection
    | ProbeRouter of sendEmptyMessageOnConnection:bool
    /// prefix messages with explicit request ID
    | RequestCorrelation of prefixExplicitRequestId:bool
    /// relax strict alternation between ZMQ.REQ and ZMQ.REP
    | RelaxStrictAlternation of strictAlternation:bool
    /// keep last message in queue (ignores high-water mark options)
    | KeepLastMessageInQueue of keepMessage:bool
    /// Sets authentication domain
    | AuthenticationDomain of domain:string
    /// Sets the maximum handshake interval
    | HandshakeInterval of delay:int<ms>
    /// If that option is set, the ROUTER socket shall hand-over the connection to the new client and disconnect the existing one.
    | RouterHandover of bool
    /// Sets the ToS fields (Differentiated services (DS) and Explicit Congestion Notification (ECN) field of the IP header. The ToS field is typically used to specify a packets priority. 
    | TypeOfService of int
    /// Sets the peer id of the next connected host, and immediately readies that connection for data transfer with the named id.
    | ConnectPeerId of byte[]
    /// Connect through a SOCKS proxy
    | SocksProxy of string
    /// do not silently drop messages if sending high water mark is reached
    | DoNoSilentlyDropMessages of bool
    /// make socket act as server or client for NULL security
    | NullSecurity
    /// make socket act as server for PLAIN security
    | PlainServer
    /// Set user name and password for client using PLAIN security
    | PlainClient of username:string * password:string
    /// Make socket act as a server for CURVE security
    | CurveServer of secretKey:byte[]  
    /// Public and private keys for client using CURVE security
    | CurveClient of publicKey:byte[] * secretKey:byte[] * serverKey:byte[] 
    /// Make socket act as a GSSAPI server.
    | GssapiServer of principal:string
    /// Make socket act as a GSSAPI client.
    | GssapiClient of principal:string * servicePrincipal:string
    /// Make socket act as a GSSAPI server, disabling encryption.
    | GssapiServerUnencripted of principal:string
    /// Make socket act as a GSSAPI client, disabling encryption.
    | GssapiClientUnencripted of principal:string * servicePrincipal:string
  
  /// Contains functions for configuring sockets and patterns for interogating sockets
  ///
  /// _(NOTE: this module is automatically opened when its parent module is opened.)_
  [<AutoOpen;CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
  module SocketOption =
    /// Sets the given `SocketOption` on the given `Socket`
    let setSocketOption socket option =
      match option with
      // general options
      | Affinity                    mask                       -> Socket.setOption socket (ZMQ.AFFINITY                 ,mask             )
      | Identity                    identifier                 -> Socket.setOption socket (ZMQ.IDENTITY                 ,identifier       )
      | Subscribe                   filter                     -> Socket.setOption socket (ZMQ.SUBSCRIBE                ,filter           )
      | Unsubscribe                 filter                     -> Socket.setOption socket (ZMQ.UNSUBSCRIBE              ,filter           )
      | Rate                        dataRate                   -> Socket.setOption socket (ZMQ.RATE                     ,dataRate         )
      | MulticastRecovery           time                       -> Socket.setOption socket (ZMQ.RECOVERY_IVL             ,time             )
      | SendBuffer                  size                       -> Socket.setOption socket (ZMQ.SNDBUF                   ,size             )
      | ReceiveBuffer               size                       -> Socket.setOption socket (ZMQ.RCVBUF                   ,size             )
      | AuthenticationDomain        domain                     -> Socket.setOption socket (ZMQ.ZAP_DOMAIN               ,domain           )
      | Backlog                     length                     -> Socket.setOption socket (ZMQ.BACKLOG                  ,length           )
      | Immediate                   immediate                  -> Socket.setOption socket (ZMQ.IMMEDIATE                ,immediate        )
      | Ipv6                        ipv6                       -> Socket.setOption socket (ZMQ.IPV6                     ,ipv6             )
      | KeepLastMessageInQueue      keep                       -> Socket.setOption socket (ZMQ.CONFLATE                 ,keep             )
      | LingerDelay                 delay                      -> Socket.setOption socket (ZMQ.LINGER                   ,delay            )
      | MaxMessageSize              size                       -> Socket.setOption socket (ZMQ.MAXMSGSIZE               ,size             )
      | MaxReconnectInterval        delay                      -> Socket.setOption socket (ZMQ.RECONNECT_IVL_MAX        ,delay            )
      | MulticastHops               hops                       -> Socket.setOption socket (ZMQ.MULTICAST_HOPS           ,hops             )
      | ProbeRouter                 router                     -> Socket.setOption socket (ZMQ.PROBE_ROUTER             ,router           )
      | ReceiveQueue                size                       -> Socket.setOption socket (ZMQ.RCVHWM                   ,size             )
      | ReceiveTimeout              delay                      -> Socket.setOption socket (ZMQ.RCVTIMEO                 ,delay            )
      | ReconnectDelay              delay                      -> Socket.setOption socket (ZMQ.RECONNECT_IVL            ,delay            )
      | RelaxStrictAlternation      relax                      -> Socket.setOption socket (ZMQ.REQ_RELAXED              ,relax            )
      | RequestCorrelation          corr                       -> Socket.setOption socket (ZMQ.REQ_CORRELATE            ,corr             )
      | ResendDuplicateMessages     resend                     -> Socket.setOption socket (ZMQ.XPUB_VERBOSE             ,resend           )
      | RouterMandatory             mandatory                  -> Socket.setOption socket (ZMQ.ROUTER_MANDATORY         ,mandatory        )
      | SendQueue                   size                       -> Socket.setOption socket (ZMQ.SNDHWM                   ,size             )
      | SendTimeout                 delay                      -> Socket.setOption socket (ZMQ.SNDTIMEO                 ,delay            )
      | TcpKeepalive                (Some keep)                -> Socket.setOption socket (ZMQ.TCP_KEEPALIVE            ,keep             )
      | TcpKeepalive                _                          -> Socket.setOption socket (ZMQ.TCP_KEEPALIVE            ,-1               )
      | TcpKeepaliveCount           count                      -> Socket.setOption socket (ZMQ.TCP_KEEPALIVE_CNT        ,count            )
      | TcpKeepaliveIdle            idle                       -> Socket.setOption socket (ZMQ.TCP_KEEPALIVE_IDLE       ,idle             )
      | TcpKeepaliveInterval        delay                      -> Socket.setOption socket (ZMQ.TCP_KEEPALIVE_INTVL      ,delay            )
      | HandshakeInterval           delay                      -> Socket.setOption socket (ZMQ.HANDSHAKE_IVL            ,delay            )
      | RouterHandover              handover                   -> Socket.setOption socket (ZMQ.ROUTER_HANDOVER          ,handover         )
      | TypeOfService               tos                        -> Socket.setOption socket (ZMQ.TOS                      ,tos              )
      | ConnectPeerId               peerId                     -> Socket.setOption socket (ZMQ.CONNECT_RID              ,peerId           )
      | SocksProxy                  proxy                      -> Socket.setOption socket (ZMQ.SOCKS_PROXY              ,proxy            )
      | DoNoSilentlyDropMessages    noDrop                     -> Socket.setOption socket (ZMQ.XPUB_NODROP              ,noDrop           )
      // security                                                                                                       
      | NullSecurity                                           -> Socket.setOption socket (ZMQ.PLAIN_SERVER             ,false            ) // using PLAIN for resetting, as there is no explicit reset otherwise
      | PlainServer                                            -> Socket.setOption socket (ZMQ.PLAIN_SERVER             ,true             )
      | PlainClient                 (unm,pwd)                  -> Socket.setOption socket (ZMQ.PLAIN_USERNAME           ,unm              )
                                                                  Socket.setOption socket (ZMQ.PLAIN_PASSWORD           ,pwd              )    
      | CurveServer                 secretKey                  -> Socket.setOption socket (ZMQ.CURVE_SERVER             ,true             )
                                                                  Socket.setOption socket (ZMQ.CURVE_SECRETKEY          ,secretKey        )
      | CurveClient (publicKey,secretKey,serverKey)            -> Socket.setOption socket (ZMQ.CURVE_SERVERKEY          ,serverKey        )
                                                                  Socket.setOption socket (ZMQ.CURVE_PUBLICKEY          ,publicKey        )
                                                                  Socket.setOption socket (ZMQ.CURVE_SECRETKEY          ,secretKey        )
      | GssapiServer                principal                  -> Socket.setOption socket (ZMQ.GSSAPI_PRINCIPAL         ,principal        )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_SERVER            ,true             )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_PLAINTEXT         ,false            )
      | GssapiServerUnencripted     principal                  -> Socket.setOption socket (ZMQ.GSSAPI_PRINCIPAL         ,principal        )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_SERVER            ,true             )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_PLAINTEXT         ,true             )
      | GssapiClient (principal, servicePrincipal)             -> Socket.setOption socket (ZMQ.GSSAPI_PRINCIPAL         ,principal        )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_SERVICE_PRINCIPAL ,servicePrincipal )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_PLAINTEXT         ,false            )
      | GssapiClientUnencripted (principal, servicePrincipal)  -> Socket.setOption socket (ZMQ.GSSAPI_PRINCIPAL         ,principal        )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_SERVICE_PRINCIPAL ,servicePrincipal )
                                                                  Socket.setOption socket (ZMQ.GSSAPI_PLAINTEXT         ,true             )

    /// Sets all the given `SocketOption`s on the given `Socket`
    let configureSocket socket socketOptions = 
      Seq.iter (setSocketOption socket) socketOptions
  
    let inline private getInt32   option socket :int32  = Socket.getOption socket option
    let inline private getBool    option socket :bool   = Socket.getOption socket option <> 0
    let inline private getInt64   option socket :int64  = Socket.getOption socket option
    let inline private getUInt64  option socket :uint64 = Socket.getOption socket option
    let inline private getBytes   option socket :byte[] = Socket.getOption socket option
    let inline private getString  option socket :string = Socket.getOption socket option
  
    let inline private getInt32WithMeasure option socket =
      socket
      |> getInt32 option
      |> LanguagePrimitives.Int32WithMeasure
  
    let inline private getInt64WithMeasure option socket =
      socket
      |> getInt64 option 
      |> LanguagePrimitives.Int64WithMeasure
       
    /// I/O thread affinity bit-mask
    let (|Affinity|) socket = getUInt64 ZMQ.AFFINITY socket
    
    /// Socket identifier
    let (|Identity|) socket = getBytes ZMQ.IDENTITY socket
  
    /// Multicast data rate
    let (|Rate|) socket : int<kb/s> = getInt32WithMeasure ZMQ.RATE socket
  
    /// Multicast recovery period
    let (|MulticastRecovery|) socket : int<ms> = getInt32WithMeasure ZMQ.RECOVERY_IVL socket
  
    /// Send-message buffer size in bytes
    let (|SendBuffer|) socket : int<Byte> = getInt32WithMeasure ZMQ.SNDBUF socket
  
    /// Receive-message buffer size in bytes
    let (|ReceiveBuffer|) socket : int<Byte> = getInt32WithMeasure ZMQ.RCVBUF socket
  
    /// More message frames available
    let (|MoreMessageFramesAvailable|) socket = getBool ZMQ.RCVMORE socket
  
    /// Socket event state, see all: Polling
    let (|Events|) socket = getInt32 ZMQ.EVENTS socket |> int16 // ZMQ actually uses an int32 in the getOptions, even though, internally only an int16 is actually used
  
    /// Socket type
    let (|SocketType|) socket : int<ZMQ.SocketType> = getInt32WithMeasure ZMQ.TYPE socket
  
    /// Pause before shutdown
    let (|LingerDelay|) socket : int<ms> = getInt32WithMeasure ZMQ.LINGER socket
  
    /// Pause before reconnect
    let (|ReconnectDelay|) socket : int<ms> = getInt32WithMeasure ZMQ.RECONNECT_IVL socket
  
    /// Maximum number of queued peers
    let (|Backlog|) socket = getInt32 ZMQ.BACKLOG socket
  
    /// Maximum reconnection interval
    let (|MaxReconnectInterval|) socket : int<ms> = getInt32WithMeasure ZMQ.RECONNECT_IVL_MAX socket
  
    /// Maximum inbound message size
    let (|MaxMessageSize|) socket : int64<Byte> = getInt64WithMeasure ZMQ.MAXMSGSIZE socket
  
    /// Maximum number of outbound queued messages
    let (|SendQueue|) socket = getInt32 ZMQ.SNDHWM socket
  
    /// Maximum number of inbound queued messages
    let (|ReceiveQueue|) socket = getInt32 ZMQ.RCVHWM socket
  
    /// Time-to-live for each multicast packet
    let (|MulticastHops|) socket : int<NetworkHop> = getInt32WithMeasure ZMQ.MULTICAST_HOPS socket
  
    /// Timeout period for inbound messages
    let (|ReceiveTimeout|) socket : int<ms> = getInt32WithMeasure ZMQ.RCVTIMEO socket
  
    /// Timeout period for outbound messages
    let (|SendTimeout|) socket : int<ms> = getInt32WithMeasure ZMQ.SNDTIMEO socket
  
    /// `bool option` indicating state of TCP_KEEPALIVE; 
    /// `None` indicates system default value
    let (|TcpKeepalive|) socket = 
      match getInt32 ZMQ.TCP_KEEPALIVE socket with
      | -1  -> None
      |  0  -> Some false
      | +1  -> Some true
      | _   -> raise (ZMQ.einval "The requested option value is invalid")
  
    /// Override OS-level TCP keep-alive
    let (|TcpKeepaliveCount|) socket = getInt32 ZMQ.TCP_KEEPALIVE_CNT socket
  
    /// Override OS-level TCP keep-alive
    let (|TcpKeepaliveIdle|) socket : int<s> = getInt32WithMeasure ZMQ.TCP_KEEPALIVE_IDLE socket
  
    /// Override OS-level TCP keep-alive
    let (|TcpKeepaliveInterval|) socket : int<s> = getInt32WithMeasure ZMQ.TCP_KEEPALIVE_INTVL socket
  
    /// Retrieve the maximum handshake interval
    let (|HandshakeInterval|) socket : int<ms> = getInt32WithMeasure ZMQ.HANDSHAKE_IVL socket
  
    /// limit queuing to only completed connections
    let (|Immediate|) socket = getBool ZMQ.IMMEDIATE socket
  
    /// true to enable IPv6 on the socket, false to restrict to only IPv4
    let (|Ipv6|) socket = getBool ZMQ.IPV6 socket
    
    /// true if the current security mechanism is NULL
    let (|NullSecurity|_|) socket = if getInt32 ZMQ.MECHANISM socket = ZMQ.SECURITY_NULL then Some () else None
    
    /// true if the current security mechanism is PLAIN and the socket is acting as a server
    let (|PlainServer|_|) socket = 
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_PLAIN ->
            if getBool ZMQ.PLAIN_SERVER socket then Some () else None
        | _ -> None

    /// true if the current security mechanism is PLAIN and the socket is acting as a client
    let (|PlainClient|_|) socket = 
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_PLAIN -> Some (getString ZMQ.PLAIN_USERNAME socket, getString ZMQ.PLAIN_PASSWORD socket)
        | _ -> None
    
    /// keep last message in queue (ignores high-water mark options)
    let (|KeepLastMessageInQueue|) socket = getBool ZMQ.CONFLATE socket
  
    /// Sets authentication domain
    let (|AuthenticationDomain|) socket = getString ZMQ.ZAP_DOMAIN socket
    
    /// true if the current security mechanism is CURVE and the socket is acting as a server
    let (|CurveServer|_|) socket =
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_CURVE ->
            let isServer = getBool ZMQ.CURVE_SERVER socket
            if isServer then 
                let secretKey : byte[] = Socket.getOptionWithBufferSize socket ZMQ.CURVE_SECRETKEY (Some 32)
                Some(secretKey)
            else None
        | _ -> None
    
    /// true if the current security mechanism is CURVE and the socket is acting as a client
    let (|CurveClient|_|) socket =
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_CURVE -> 
            let publicKey : byte[] = Socket.getOptionWithBufferSize socket ZMQ.CURVE_PUBLICKEY (Some 32)
            let secretKey : byte[] = Socket.getOptionWithBufferSize socket ZMQ.CURVE_SECRETKEY (Some 32)
            let serverKey : byte[] = Socket.getOptionWithBufferSize socket ZMQ.CURVE_SERVERKEY (Some 32)
            Some (publicKey, secretKey, serverKey)
        | _ -> None

    /// Retrieves the last endpoint bound for TCP and IPC transports
    let (|LastEndpointAddress|) socket = getString ZMQ.LAST_ENDPOINT socket
   
    /// Retrieves the TypeOfService option for the socket
    let (|TypeOfService|) socket = getInt32 ZMQ.TOS socket

    /// Retrieves SOCKS proxy address
    let (|SocksProxy|) socket = getString ZMQ.SOCKS_PROXY socket
    
    /// true if the current security mechanism is GSSAPI and the socket is acting as a server
    let (|GssapiServer|_|) socket = 
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_GSSAPI -> 
            let isServer = getBool ZMQ.GSSAPI_SERVER socket
            if isServer then 
                let isEncrypted = getBool ZMQ.GSSAPI_PLAINTEXT socket |> not
                if isEncrypted then getString ZMQ.GSSAPI_PRINCIPAL socket |> Some else None
            else None
        | _ -> None
        
    /// true if the current security mechanism is GSSAPI and the socket is acting as a server
    let (|GssapiServerUnencripted|_|) socket = 
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_GSSAPI ->
            let isServer = getBool ZMQ.GSSAPI_SERVER socket
            if isServer then
                let isPlain = getBool ZMQ.GSSAPI_PLAINTEXT socket
                if isPlain then getString ZMQ.GSSAPI_PRINCIPAL socket |> Some else None
            else None
        | _ -> None
    
    /// true if the current security mechanism is GSSAPI and the socket is acting as a Client
    let (|GssapiClient|_|) socket = 
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_GSSAPI ->
         
            let isEncrypted = getBool ZMQ.GSSAPI_PLAINTEXT socket |> not
            if isEncrypted then 
                let principal = getString ZMQ.GSSAPI_PRINCIPAL socket
                let servicePrincipal = getString ZMQ.GSSAPI_SERVICE_PRINCIPAL socket
                Some (principal, servicePrincipal)
            else None
        | _ -> None
        
    /// true if the current security mechanism is GSSAPI and the socket is acting as a Client
    let (|GssapiClientUnencripted|_|) socket = 
        match getInt32 ZMQ.MECHANISM socket with
        | ZMQ.SECURITY_GSSAPI -> 
            let isPlain = getBool ZMQ.GSSAPI_PLAINTEXT socket
            if isPlain then 
                let principal = getString ZMQ.GSSAPI_PRINCIPAL socket
                let servicePrincipal = getString ZMQ.GSSAPI_SERVICE_PRINCIPAL socket
                Some (principal, servicePrincipal)
            else None
        | _ -> None
