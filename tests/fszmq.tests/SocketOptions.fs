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

            let actual = (%patternMatch) socket
            actual = expected @>

  let testSetter socketType option =
    use context = new Context()
    use socket = Context.newSocket context socketType
    setSocketOption socket option

  let testGetter expected patternMatch =
    use context = new Context()
    use socket = Context.newSocket context ZMQ.PUB
    test <@ let actual = (%patternMatch) socket
            actual = expected @>

  [<Test>]
  let ``setting options and reading them are inverses: Affinity`` () =
    <@ fun (Affinity actual) -> Affinity actual @>
    |> testInverses <| Affinity 0x123UL
  
  [<Test>]
  let ``setting options and reading them are inverses: Identity`` () =
    <@ fun (Identity actual) -> Identity actual @>
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
    <@ fun (Rate actual) -> Rate actual @>
    |> testInverses <| Rate 123<kb/s>

  [<Test>]
  let ``setting options and reading them are inverses: MulticastRecovery`` () =
    <@ fun (MulticastRecovery actual) -> MulticastRecovery actual @>
    |> testInverses <| MulticastRecovery 123<ms>

  [<Test>]
  let ``setting options and reading them are inverses: SendBuffer`` () =
    <@ fun (SendBuffer actual) -> SendBuffer actual @>
    |> testInverses <| SendBuffer 123<B>

  [<Test>]
  let ``setting options and reading them are inverses: ReceiveBuffer`` () =
    <@ fun (ReceiveBuffer actual) -> ReceiveBuffer actual @>
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
    <@ fun (AuthenticationDomain actual) -> AuthenticationDomain actual @>
    |> testInverses <| AuthenticationDomain "my domain"

  [<Test>]
  let ``setting options and reading them are inverses: Backlog`` () =
    <@ fun (Backlog actual) -> Backlog actual @>
    |> testInverses <| Backlog 123

  [<Test>]
  let ``setting options and reading them are inverses: Immediate true`` () =
    <@ fun (Immediate actual) -> Immediate actual @>
    |> testInverses <| Immediate true

  [<Test>]
  let ``setting options and reading them are inverses: Immediate false`` () =
    <@ fun (Immediate actual) -> Immediate actual @>
    |> testInverses <| Immediate false
  
  [<Test>]
  let ``setting options and reading them are inverses: Ipv6 true`` () =
    <@ fun (Ipv6 actual) -> Ipv6 actual @>
    |> testInverses <| Ipv6 true

  [<Test>]
  let ``setting options and reading them are inverses: Ipv6 false`` () =
    <@ fun (Ipv6 actual) -> Ipv6 actual @>
    |> testInverses <| Ipv6 false

  [<Test>]
  let ``setting options and reading them are inverses: KeepLastMessageInQueue true`` () =
    <@ fun (KeepLastMessageInQueue actual) -> KeepLastMessageInQueue actual @>
    |> testInverses <| KeepLastMessageInQueue true
  
  [<Test>]
  let ``setting options and reading them are inverses: KeepLastMessageInQueue false`` () =
    <@ fun (KeepLastMessageInQueue actual) -> KeepLastMessageInQueue actual @>
    |> testInverses <| KeepLastMessageInQueue false

  [<Test>]
  let ``setting options and reading them are inverses: LingerDelay`` () =
    <@ fun (LingerDelay actual) -> LingerDelay actual @>
    |> testInverses <| LingerDelay 123<ms>

  [<Test>]
  let ``setting options and reading them are inverses: MaxMessageSize`` () =
    <@ fun (MaxMessageSize actual) -> MaxMessageSize actual @>
    |> testInverses <| MaxMessageSize 123L<B>

  [<Test>]
  let ``setting options and reading them are inverses: MaxReconnectInterval`` () =
    <@ fun (MaxReconnectInterval actual) -> MaxReconnectInterval actual @>
    |> testInverses <| MaxReconnectInterval 123<ms>

  [<Test>]
  let ``setting options and reading them are inverses: MulticastHops`` () =
    <@ fun (MulticastHops actual) -> MulticastHops actual @>
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
    <@ fun (ReceiveQueue actual) -> ReceiveQueue actual @>
    |> testInverses <| ReceiveQueue 123

  [<Test>]
  let ``setting options and reading them are inverses: ReceiveTimeout`` () =
    <@ fun (ReceiveTimeout actual) -> ReceiveTimeout actual @>
    |> testInverses <| ReceiveTimeout 123<ms>
  
  [<Test>]
  let ``setting options and reading them are inverses: ReconnectDelay`` () =
    <@ fun (ReconnectDelay actual) -> ReconnectDelay actual @>
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
  let ``setting options does not throw: RouterHandover false`` () =
    RouterHandover false
    |> testSetter ZMQ.ROUTER

  [<Test>]
  let ``setting options does not throw: RouterHandover true`` () =
    RouterHandover true
    |> testSetter ZMQ.ROUTER

  [<Test>]
  let ``setting options and reading them are inverses: SendQueue`` () =
    <@ fun (SendQueue actual) -> SendQueue actual @>
    |> testInverses <| SendQueue 123

  [<Test>]
  let ``setting options and reading them are inverses: SendTimeout`` () =
    <@ fun (SendTimeout actual) -> SendTimeout actual @>
    |> testInverses <| SendTimeout 123<ms>
  
  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepalive None`` () =
    <@ fun (TcpKeepalive actual) -> TcpKeepalive actual @>
    |> testInverses <| TcpKeepalive None

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepalive (Some false)`` () =
    <@ fun (TcpKeepalive actual) -> TcpKeepalive actual @>
    |> testInverses <| TcpKeepalive (Some false)

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepalive (Some true)`` () =
    <@ fun (TcpKeepalive actual) -> TcpKeepalive actual @>
    |> testInverses <| TcpKeepalive (Some true)
  
  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepaliveCount`` () =
    <@ fun (TcpKeepaliveCount actual) -> TcpKeepaliveCount actual @>
    |> testInverses <| TcpKeepaliveCount 123

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepaliveIdle`` () =
    <@ fun (TcpKeepaliveIdle actual) -> TcpKeepaliveIdle actual @>
    |> testInverses <| TcpKeepaliveIdle 123<s>

  [<Test>]
  let ``setting options and reading them are inverses: TcpKeepaliveInterval`` () =
    <@ fun (TcpKeepaliveInterval actual) -> TcpKeepaliveInterval actual @>
    |> testInverses <| TcpKeepaliveInterval 123<s>

  [<Test>]
  let ``setting options and reading them are inverses: HandshakeInterval`` () =
    <@ fun (HandshakeInterval actual) -> HandshakeInterval actual @>
    |> testInverses <| HandshakeInterval 123<ms>
    
  let parseSecurity = 
    <@ function
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

  [<Test>]
  let ``getting options does not throw: LastEndpointAddress`` () =
    <@ fun (LastEndpointAddress address) -> address @>
    |> testGetter ""