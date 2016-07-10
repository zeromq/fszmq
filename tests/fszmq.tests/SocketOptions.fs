namespace fszmq.tests
#nowarn "57" (* Disable "experimental" warnings from Options module *)
open FsCheck
open fszmq
open fszmq.Options
open NUnit.Framework
open Swensen.Unquote
open System


[<TestFixture>]
module Options =
  
  [<Test>]
  let ``SocketType pattern should equal ZMQ.TYPE option`` () = 
    test <@ use context = new Context ()
            seq { 0 .. 11 } |> Seq.forall (fun value ->
              // construct socket
              let socketType  = value * 1<ZMQ.SocketType>
              use socket      = Context.newSocket context socketType
              // extract info
              let expected = Socket.getOption socket ZMQ.TYPE
              let (SocketType actual) = socket
              // evaluate
              expected = actual && actual = socketType) @>

  let testInverses patternMatch expected =
    use context = new Context()
    use socket = Context.newSocket context ZMQ.PUB
    test <@ expected |> setSocketOption socket

            let actual = %patternMatch socket
            actual = expected @>

  let testSetter socketType option =
    use context = new Context()
    use socket = Context.newSocket context socketType
    setSocketOption socket option

  let testGetter expected patternMatch =
    test <@ use context = new Context()
            use socket = Context.newSocket context ZMQ.PUB
            let actual = (%patternMatch) socket
            actual = expected @>


  [<Test>]
  let ``setting options and reading them are inverses: Affinity`` () =
    fun socket -> <@ let (Affinity actual) = socket in Affinity actual @>
    |> testInverses <| Affinity 0x123UL
  
  [<Test>]
  let ``setting options and reading them are inverses: Identity`` () =
    fun socket -> <@ let (Identity actual) = socket in Identity actual @>
    |> testInverses <| Identity "My Identity"B

  [<Test>]
  let ``setting options does not throw: Subscribe "prefix"B`` () =
    Subscribe "prefix"B
    |> testSetter ZMQ.SUB

  [<Test>]
  let ``setting options does not throw: Unsubscribe "prefix"B`` () =
    Unsubscribe "prefix"B
    |> testSetter ZMQ.SUB

  [<Test>]
  let ``setting options and reading them are inverses: Rate`` () =
    fun socket -> <@ let (Rate actual) = socket in Rate actual @>
    |> testInverses <| Rate 123<kb/s>

  [<Test>]
  let ``setting options and reading them are inverses: MulticastRecovery`` () =
    fun socket -> <@ let (MulticastRecovery actual) = socket in MulticastRecovery actual @>
    |> testInverses <| MulticastRecovery 123<ms>

  [<Test>]
  let ``setting options and reading them are inverses: SendBuffer`` () =
    fun socket -> <@ let (SendBuffer actual) = socket in SendBuffer actual @>
    |> testInverses <| SendBuffer 123<B>

  [<Test>]
  let ``setting options and reading them are inverses: ReceiveBuffer`` () =
    fun socket -> <@ let (ReceiveBuffer actual) = socket in ReceiveBuffer actual @>
    |> testInverses <| ReceiveBuffer 123<B>

  [<Test>]
  let ``getting options does not throw: MoreMessageFramesAvailable`` () =
    <@ fun (MoreMessageFramesAvailable moreMessages) -> moreMessages @>
    |> testGetter false

  [<Test>]
  let ``getting options does not throw: Events`` () =
    <@ fun (Events polling) -> polling @>
    |> testGetter ZMQ.POLLOUT

  [<Test>]
  let ``setting options and reading them are inverses: AuthenticationDomain`` () =
    fun socket -> <@ let (AuthenticationDomain actual) = socket in AuthenticationDomain actual @>
    |> testInverses <| AuthenticationDomain "my domain"

  [<Test>]
  let ``setting options and reading them are inverses: Backlog`` () =
    fun socket -> <@ let (Backlog actual) = socket in Backlog actual @>
    |> testInverses <| Backlog 123

  [<Test>]
  let ``setting options and reading them are inverses: Immediate true`` () =
    fun socket -> <@ let (Immediate actual) = socket in Immediate actual @>
    |> testInverses <| Immediate true

  [<Test>]
  let ``setting options and reading them are inverses: Immediate false`` () =
    fun socket -> <@ let (Immediate actual) = socket in Immediate actual @>
    |> testInverses <| Immediate false
  
  [<Test>]
  let ``setting options and reading them are inverses: Ipv6 true`` () =
    fun socket -> <@ let (Ipv6 actual) = socket in Ipv6 actual @>
    |> testInverses <| Ipv6 true

  [<Test>]
  let ``setting options and reading them are inverses: Ipv6 false`` () =
    fun socket -> <@ let (Ipv6 actual) = socket in Ipv6 actual @>
    |> testInverses <| Ipv6 false

  [<Test>]
  let ``setting options and reading them are inverses: KeepLastMessageInQueue true`` () =
    fun socket -> <@ let (KeepLastMessageInQueue actual) = socket in KeepLastMessageInQueue actual @>
    |> testInverses <| KeepLastMessageInQueue true
  
  [<Test>]
  let ``setting options and reading them are inverses: KeepLastMessageInQueue false`` () =
    fun socket -> <@ let (KeepLastMessageInQueue actual) = socket in KeepLastMessageInQueue actual @>
    |> testInverses <| KeepLastMessageInQueue false

  [<Test>]
  let ``setting options and reading them are inverses: LingerDelay`` () =
    fun socket -> <@ let (LingerDelay actual) = socket in LingerDelay actual @>
    |> testInverses <| LingerDelay 123<ms>

  [<Test>]
  let ``setting options and reading them are inverses: MaxMessageSize`` () =
    fun socket -> <@ let (MaxMessageSize actual) = socket in MaxMessageSize actual @>
    |> testInverses <| MaxMessageSize 123L<B>

  [<Test>]
  let ``setting options and reading them are inverses: MaxReconnectInterval`` () =
    fun socket -> <@ let (MaxReconnectInterval actual) = socket in MaxReconnectInterval actual @>
    |> testInverses <| MaxReconnectInterval 123<ms>

  [<Test>]
  let ``setting options and reading them are inverses: MulticastHops`` () =
    fun socket -> <@ let (MulticastHops actual) = socket in MulticastHops actual @>
    |> testInverses <| MulticastHops 123<NetworkHop>

  [<Test>]
  let ``setting options does not throw: ProbeRouter true`` () =
    ProbeRouter true
    |> testSetter ZMQ.REQ

  [<Test>]
  let ``setting options does not throw: ProbeRouter false`` () =
    ProbeRouter false
    |> testSetter ZMQ.REQ
  
  [<Test>]
  let ``setting options and reading them are inverses: ReceiveQueue`` () =
    fun socket -> <@ let (ReceiveQueue actual) = socket in ReceiveQueue actual @>
    |> testInverses <| ReceiveQueue 123

  [<Test>]
  let ``setting options and reading them are inverses: ReceiveTimeout`` () =
    fun socket -> <@ let (ReceiveTimeout actual) = socket in ReceiveTimeout actual @>
    |> testInverses <| ReceiveTimeout 123<ms>
  
  [<Test>]
  let ``setting options and reading them are inverses: ReconnectDelay`` () =
    fun socket -> <@ let (ReconnectDelay actual) = socket in ReconnectDelay actual @>
    |> testInverses <| ReconnectDelay 123<ms>

  [<Test>]
  let ``setting options does not throw: RelaxStrictAlternation true`` () =
    RelaxStrictAlternation true
    |> testSetter ZMQ.REQ

  [<Test>]
  let ``setting options does not throw: RelaxStrictAlternation false`` () =
    RelaxStrictAlternation false
    |> testSetter ZMQ.REQ

  [<Test>]
  let ``setting options does not throw: RequestCorrelation true`` () =
    RequestCorrelation true
    |> testSetter ZMQ.REQ

  [<Test>]
  let ``setting options does not throw: RequestCorrelation false`` () =
    RequestCorrelation false
    |> testSetter ZMQ.REQ

  [<Test>]
  let ``setting options does not throw: ResendDuplicateMessages true`` () =
    ResendDuplicateMessages true
    |> testSetter ZMQ.XPUB

  [<Test>]
  let ``setting options does not throw: ResendDuplicateMessages false`` () =
    ResendDuplicateMessages false
    |> testSetter ZMQ.XPUB 
 
  [<Test>]
  let ``setting options does not throw: RouterMandatory true`` () =
    RouterMandatory true
    |> testSetter ZMQ.ROUTER

  [<Test>]
  let ``setting options does not throw: RouterMandatory false`` () =
    RouterMandatory false
    |> testSetter ZMQ.ROUTER

  [<Test>]
  let ``setting options and reading them are inverses: SendQueue`` () =
    fun socket -> <@ let (SendQueue actual) = socket in SendQueue actual @>
    |> testInverses <| SendQueue 123

  [<Test>]
  let ``setting options and reading them are inverses: SendTimeout`` () =
    fun socket -> <@ let (SendTimeout actual) = socket in SendTimeout actual @>
    |> testInverses <| SendTimeout 123<ms>
  
  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepalive None`` () =
    fun socket -> <@ let (TcpKeepalive actual) = socket in TcpKeepalive actual @>
    |> testInverses <| TcpKeepalive None

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepalive (Some false)`` () =
    fun socket -> <@ let (TcpKeepalive actual) = socket in TcpKeepalive actual @>
    |> testInverses <| TcpKeepalive (Some false)

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepalive (Some true)`` () =
    fun socket -> <@ let (TcpKeepalive actual) = socket in TcpKeepalive actual @>
    |> testInverses <| TcpKeepalive (Some true)
  
  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepaliveCount`` () =
    fun socket -> <@ let (TcpKeepaliveCount actual) = socket in TcpKeepaliveCount actual @>
    |> testInverses <| TcpKeepaliveCount 123

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepaliveIdle`` () =
    fun socket -> <@ let (TcpKeepaliveIdle actual) = socket in TcpKeepaliveIdle actual @>
    |> testInverses <| TcpKeepaliveIdle 123<s>

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepaliveInterval`` () =
    fun socket -> <@ let (TcpKeepaliveInterval actual) = socket in TcpKeepaliveInterval actual @>
    |> testInverses <| TcpKeepaliveInterval 123<s>

  [<Test>]
  let ``setting options and reading them are inverses: HandshakeInterval`` () =
    fun socket -> <@ let (HandshakeInterval actual) = socket in HandshakeInterval actual @>
    |> testInverses <| HandshakeInterval 123<ms>
    
  let parseSecurity socket = 
    <@ match socket with
       | NullSecurity -> NullSecurity
       | PlainServer -> PlainServer
       | PlainClient (unm, pwd) -> PlainClient(unm, pwd)
       | CurveServer secretKey -> CurveServer secretKey
       | CurveClient (publicKey, secretKey, serverKey) -> CurveClient (publicKey, secretKey, serverKey)
       | _ -> failwith "unknown security mechanism" @>  

  [<Test>]
  let ``setting options and reading them are inverses: NullSecurity`` () =
    parseSecurity
    |> testInverses <| NullSecurity
    
  [<Test>]
  let ``setting options and reading them are inverses: PlainServer`` () =
    parseSecurity
    |> testInverses <| PlainServer    

  [<Test>]
  let ``setting options and reading them are inverses: PlainClient`` () =
    parseSecurity
    |> testInverses <| PlainClient ("myUserName", "supersecurepassword")

  /// Well-known keys from the ZMQ documentation.
  module private CurveKeys =
    let clientPublicKey = Z85.decode "Yne@$w-vo<fVvi]a<NY6T1ed:M$fCG*[IaLV{hID"
    let clientSecretKey = Z85.decode "D:)Q[IlAW!ahhC2ac:9*A}h:p?([4%wOTJ%JR%cs"

    let serverPublicKey = Z85.decode "rq:rM>}U?@Lns47E1%kR.o@n%FcmmsL/@{H8]yf7"
    let serverSecretKey = Z85.decode "JTKVSB%%)wK0E.X)V>+}o?pNmC{O&4W4b!Ni{Lh6"

  [<Test>]
  let ``setting options and reading them are inverses: CurveServer`` () =   
    parseSecurity
    |> testInverses <| CurveServer CurveKeys.serverSecretKey

  [<Test>]
  let ``setting options and reading them are inverses: CurveClient`` () =   
    parseSecurity
    |> testInverses <| CurveClient(CurveKeys.clientPublicKey, CurveKeys.clientSecretKey, CurveKeys.serverPublicKey)

