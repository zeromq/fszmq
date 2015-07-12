module fszmq.Options

open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

type [<Measure>] bit
type [<Measure>] b = bit
type [<Measure>] kilobit
type [<Measure>] kb = kilobit
type [<Measure>] millisecond
type [<Measure>] ms = millisecond
type [<Measure>] Byte
type [<Measure>] B = Byte
type [<Measure>] s = Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols.s
type [<Measure>] NetworkHop

type SecurityMechanism =
| Null
| Plain
| Curve

/// Socket options
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
/// More message frames available
| MoreMessageFramesAvailable of bool
/// Socket event state, see all: Polling
| Events of polling:int
/// Socket type
| SocketType of socketType:int<ZMQ.SocketType>
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
| TcpKeepalive of level:int //TODO: verify this
/// Override OS-level TCP keep-alive
| TcpKeepaliveCount of count:int //TODO: verify this
/// Override OS-level TCP keep-alive
| TcpKeepaliveIdle of idle:int //TODO: verify this
/// Override OS-level TCP keep-alive
| TcpKeepaliveInterval of delay:int<ms> //TODO: verify this
/// TCP/IP filters
| TcpAcceptFilters of filters:byte[]
/// limit queuing to only completed connections
| Immediate of queueCompletedOnly:bool
/// resend duplicate messages
| ResendDuplicateMessages of resendDuplicate:bool
/// true to enable IPv6 on the socket, false to restrict to only IPv4
| Ipv6 of ipv6:bool
/// make socket act as server for PLAIN security
| PlainServer of username:string * password:string
/// make socket act as server for CURVE security
| CurveServerString of longTermPublicKey:string * longTermSecretKey:string * serverKey:string 
/// make socket act as server for CURVE security
| CurveServer of longTermPublicKey:byte[] * longTermSecretKey:byte[] * serverKey:byte[] 
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

[<AutoOpen; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SocketOption =
      
  let setSocketOption (socket:Socket) option =
    let setter socketOption write (size, buffer) =
      write buffer
      let okay = C.zmq_setsockopt(socket.Handle,int socketOption,buffer,size)
      if  okay <> 0 then ZMQ.error()
    let useBuffer size option write value =
        useBuffer size (setter option (write value))
    match option with
    | Affinity mask ->                      useBuffer sizeof<uint64> ZMQ.AFFINITY writeUInt64 mask
    | Identity identifier ->                useBuffer identifier.Length ZMQ.IDENTITY writeBytes identifier
    | Subscribe filter ->                   useBuffer filter.Length ZMQ.SUBSCRIBE writeBytes filter
    | Unsubscribe filter ->                 useBuffer filter.Length ZMQ.UNSUBSCRIBE writeBytes filter
    | Rate dataRate ->                      useBuffer sizeof<int32> ZMQ.RATE writeInt32 (int dataRate)
    | MulticastRecovery time ->             useBuffer sizeof<int32> ZMQ.RECOVERY_IVL writeInt32 (int time)
    | SendBuffer size ->                    useBuffer sizeof<int32> ZMQ.SNDBUF writeInt32 (int size)
    | ReceiveBuffer size ->                 useBuffer sizeof<int32> ZMQ.RCVBUF writeInt32 (int size)
    | MoreMessageFramesAvailable more ->    useBuffer sizeof<int32> ZMQ.RCVMORE writeBool more
    | Events events ->                      useBuffer sizeof<int32> ZMQ.EVENTS writeInt32 (int events)    
    | SocketType socketType ->              useBuffer sizeof<int32> ZMQ.TYPE writeInt32 (int socketType)    
    | AuthenticationDomain domain ->        useBuffer domain.Length ZMQ.ZAP_DOMAIN writeString domain
    | Backlog length ->                     useBuffer sizeof<int32> ZMQ.BACKLOG writeInt32 length 
    | CurveServer (publicKey, secretKey, serverKey) ->
        useBuffer sizeof<int32> ZMQ.CURVE_SERVER writeInt32 1
        useBuffer publicKey.Length ZMQ.CURVE_PUBLICKEY writeBytes publicKey
        useBuffer serverKey.Length ZMQ.CURVE_SECRETKEY writeBytes secretKey
        useBuffer serverKey.Length ZMQ.CURVE_SERVERKEY writeBytes serverKey
    | CurveServerString (publicKey, secretKey, serverKey) ->
        useBuffer sizeof<int32> ZMQ.CURVE_SERVER writeInt32 1
        useBuffer publicKey.Length ZMQ.CURVE_PUBLICKEY writeString publicKey
        useBuffer serverKey.Length ZMQ.CURVE_SECRETKEY writeString secretKey
        useBuffer serverKey.Length ZMQ.CURVE_SERVERKEY writeString serverKey
    | Immediate true ->                     useBuffer sizeof<int32> ZMQ.IMMEDIATE writeInt32 1
    | Immediate false ->                    useBuffer sizeof<int32> ZMQ.IMMEDIATE writeInt32 0
    | Ipv6 true ->                          useBuffer sizeof<int32> ZMQ.IPV6 writeInt32 1
    | Ipv6 false ->                         useBuffer sizeof<int32> ZMQ.IPV6 writeInt32 0
    | KeepLastMessageInQueue true ->        useBuffer sizeof<int32> ZMQ.CONFLATE writeInt32 1
    | KeepLastMessageInQueue false ->       useBuffer sizeof<int32> ZMQ.CONFLATE writeInt32 0
    | LingerDelay delay ->                  useBuffer sizeof<int32> ZMQ.LINGER writeInt32 (int delay)
    | MaxMessageSize size ->                useBuffer sizeof<int64> ZMQ.MAXMSGSIZE writeInt64 (int64 size)
    | MaxReconnectInterval delay ->         useBuffer sizeof<int32> ZMQ.TCP_KEEPALIVE_INTVL writeInt32 (int delay)
    | MulticastHops hops ->                 useBuffer sizeof<int32> ZMQ.MULTICAST_HOPS writeInt32 (int hops)
    | PlainServer (username, password) ->
        useBuffer sizeof<int32> ZMQ.PLAIN_SERVER writeInt32 1
        useBuffer username.Length ZMQ.PLAIN_USERNAME writeString username
        useBuffer password.Length ZMQ.PLAIN_PASSWORD writeString password
    | ProbeRouter true ->                   useBuffer sizeof<int32> ZMQ.PROBE_ROUTER writeInt32 1
    | ProbeRouter false ->                  useBuffer sizeof<int32> ZMQ.PROBE_ROUTER writeInt32 0
    | ReceiveQueue size ->                  useBuffer sizeof<int32> ZMQ.RCVHWM writeInt32 (int size)
    | ReceiveTimeout delay ->               useBuffer sizeof<int32> ZMQ.RCVTIMEO writeInt32 (int delay)
    | ReconnectDelay delay ->               useBuffer sizeof<int32> ZMQ.RECONNECT_IVL writeInt32 (int delay)
    | RelaxStrictAlternation true ->        useBuffer sizeof<int32> ZMQ.REQ_RELAXED writeInt32 1
    | RelaxStrictAlternation false ->       useBuffer sizeof<int32> ZMQ.REQ_RELAXED writeInt32 0
    | RequestCorrelation true ->            useBuffer sizeof<int32> ZMQ.REQ_CORRELATE writeInt32 1
    | RequestCorrelation false ->           useBuffer sizeof<int32> ZMQ.REQ_CORRELATE writeInt32 0
    | ResendDuplicateMessages true ->       useBuffer sizeof<int32> ZMQ.XPUB_VERBOSE writeInt32 1
    | ResendDuplicateMessages false ->      useBuffer sizeof<int32> ZMQ.XPUB_VERBOSE writeInt32 0
    | RouterMandatory true ->               useBuffer sizeof<int32> ZMQ.ROUTER_MANDATORY writeInt32 1
    | RouterMandatory false ->              useBuffer sizeof<int32> ZMQ.ROUTER_MANDATORY writeInt32 0
    | SendQueue size ->                     useBuffer sizeof<int32> ZMQ.SNDHWM writeInt32 size
    | SendTimeout delay ->                  useBuffer sizeof<int32> ZMQ.SNDTIMEO writeInt32 (int delay)
    | TcpAcceptFilters filters ->           useBuffer filters.Length ZMQ.TCP_ACCEPT_FILTER writeBytes filters
    | TcpKeepalive level ->                 useBuffer sizeof<int32> ZMQ.TCP_KEEPALIVE writeInt32 level
    | TcpKeepaliveCount count ->            useBuffer sizeof<int32> ZMQ.TCP_KEEPALIVE_CNT writeInt32 count
    | TcpKeepaliveIdle idle ->              useBuffer sizeof<int32> ZMQ.TCP_KEEPALIVE_IDLE writeInt32 idle
    | TcpKeepaliveInterval delay ->         useBuffer sizeof<int32> ZMQ.TCP_KEEPALIVE_INTVL writeInt32 (int delay)

  let configureSocket socket socketOptions =
    Seq.iter (setSocketOption socket) socketOptions

  let inline private getInt32 option socket : int32 = Socket.getOption socket option
  let inline private getBool option socket = Socket.getOption socket option <> 0
  let inline private getInt64 option socket : int64 = Socket.getOption socket option
  let inline private getUInt64 option socket : uint64 = Socket.getOption socket option
  let inline private getBytes option socket : byte[] = Socket.getOption socket option
  let inline private getString option socket : string = Socket.getOption socket option

  let inline private getInt32WithMeasure option socket =
    getInt32 option socket
    |> LanguagePrimitives.Int32WithMeasure

  let inline private getInt64WithMeasure option socket =
    getInt64 option socket
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
  let (|Events|) socket = getInt32 ZMQ.EVENTS socket

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

  /// true to error on unroutable messages, false to silently ignore
  let (|RouterMandatory|) socket = getBool ZMQ.ROUTER_MANDATORY socket

  /// Override OS-level TCP keep-alive
  let (|TcpKeepalive|) socket = getInt32 ZMQ.TCP_KEEPALIVE socket // TODO: verify this

  /// Override OS-level TCP keep-alive
  let (|TcpKeepaliveCount|) socket = getInt32 ZMQ.TCP_KEEPALIVE_CNT socket // TODO: verify this

  /// Override OS-level TCP keep-alive
  let (|TcpKeepaliveIdle|) socket = getInt32 ZMQ.TCP_KEEPALIVE_IDLE socket //TODO: verify this

  /// Override OS-level TCP keep-alive
  let (|TcpKeepaliveInterval|) socket : int<ms> = getInt32WithMeasure ZMQ.TCP_KEEPALIVE_INTVL socket //TODO: verify this

  /// TCP/IP filters
  let (|TcpAcceptFilters|) socket = getBytes ZMQ.TCP_ACCEPT_FILTER socket

  /// limit queuing to only completed connections
  let (|Immediate|) socket = getBool ZMQ.IMMEDIATE socket

  /// resend duplicate messages
  let (|ResendDuplicateMessages|) socket = getBool ZMQ.XPUB_VERBOSE socket

  /// true to enable IPv6 on the socket, false to restrict to only IPv4
  let (|Ipv6|) socket = getBool ZMQ.IPV6 socket

  /// make socket act as server for PLAIN security
  let (|NoSecurity|Curve|Plain|) socket =
    match getInt32 ZMQ.MECHANISM socket with
    | 1 -> Plain
    | 2 -> Curve
    | _ -> NoSecurity

  /// automatically send an empty message on new connection
  let (|ProbeRouter|) socket = getBool ZMQ.PROBE_ROUTER socket

  /// prefix messages with explicit request ID
  let (|RequestCorrelation|) socket = getBool ZMQ.REQ_CORRELATE socket

  /// relax strict alternation between ZMQ.REQ and ZMQ.REP
  let (|RelaxStrictAlternation|) socket = getBool ZMQ.REQ_RELAXED socket

  /// keep last message in queue (ignores high-water mark options)
  let (|KeepLastMessageInQueue|) socket = getBool ZMQ.CONFLATE socket

  /// Sets authentication domain
  let (|AuthenticationDomain|) socket = getString ZMQ.ZAP_DOMAIN socket