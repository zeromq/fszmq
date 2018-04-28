namespace fszmq.tests

open Expecto
open fszmq
open fszmq.Options
open Swensen.Unquote

module Options =
  
  [<Tests>]
  let tests =
    testCase "SocketType pattern should equal ZMQ.TYPE option" <| fun () ->
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
  
  let assertHas cap =
    match ZMQ.has cap with
    | Supported (_, true) -> ()
    | Supported (_, false) -> skiptestf "capability missing: %A" cap
    | Unknown -> skiptestf "Unable to confirm if the capability exists: %A" cap

  [<Tests>]
  let props =
    testList "properties" [
      testCase "setting options and reading them are inverses: Affinity" <| fun () ->
        <@ fun (Affinity actual) -> Affinity actual @>
        |> testInverses <| Affinity 0x123UL
  
      testCase "setting options and reading them are inverses: Identity" <| fun () ->
        <@ fun (Identity actual) -> Identity actual @>
        |> testInverses <| Identity "My Identity"B

      testCase "setting options does not throw: Subscribe \"prefix\"B" <| fun () ->
        Subscribe "prefix"B
        |> testSetter ZMQ.SUB

      testCase "setting options does not throw: Unsubscribe \"prefix\"B" <| fun () ->
        Unsubscribe "prefix"B
        |> testSetter ZMQ.SUB

      testCase "setting options and reading them are inverses: Rate" <| fun () ->
        <@ fun (Rate actual) -> Rate actual @>
        |> testInverses <| Rate 123<kb/s>

      testCase "setting options and reading them are inverses: MulticastRecovery" <| fun () ->
        <@ fun (MulticastRecovery actual) -> MulticastRecovery actual @>
        |> testInverses <| MulticastRecovery 123<ms>

      testCase "setting options and reading them are inverses: SendBuffer" <| fun () ->
        <@ fun (SendBuffer actual) -> SendBuffer actual @>
        |> testInverses <| SendBuffer 123<B>

      testCase "setting options and reading them are inverses: ReceiveBuffer" <| fun () ->
        <@ fun (ReceiveBuffer actual) -> ReceiveBuffer actual @>
        |> testInverses <| ReceiveBuffer 123<B>

      testCase "getting options does not throw: MoreMessageFramesAvailable" <| fun () ->
        <@ fun (MoreMessageFramesAvailable moreMessages) -> moreMessages @>
        |> testGetter false

      testCase "getting options does not throw: Events" <| fun () ->
        <@ fun (Events polling) -> polling @>
        |> testGetter ZMQ.POLLOUT

      testCase "setting options and reading them are inverses: AuthenticationDomain" <| fun () ->
        <@ fun (AuthenticationDomain actual) -> AuthenticationDomain actual @>
        |> testInverses <| AuthenticationDomain "my domain"

      testCase "setting options and reading them are inverses: Backlog" <| fun () ->
        <@ fun (Backlog actual) -> Backlog actual @>
        |> testInverses <| Backlog 123

      testCase "setting options and reading them are inverses: Immediate true" <| fun () ->
        <@ fun (Immediate actual) -> Immediate actual @>
        |> testInverses <| Immediate true

      testCase "setting options and reading them are inverses: Immediate false" <| fun () ->
        <@ fun (Immediate actual) -> Immediate actual @>
        |> testInverses <| Immediate false
      
      testCase "setting options and reading them are inverses: Ipv6 true" <| fun () ->
        <@ fun (Ipv6 actual) -> Ipv6 actual @>
        |> testInverses <| Ipv6 true

      testCase "setting options and reading them are inverses: Ipv6 false" <| fun () ->
        <@ fun (Ipv6 actual) -> Ipv6 actual @>
        |> testInverses <| Ipv6 false

      testCase "setting options and reading them are inverses: KeepLastMessageInQueue true" <| fun () ->
        <@ fun (KeepLastMessageInQueue actual) -> KeepLastMessageInQueue actual @>
        |> testInverses <| KeepLastMessageInQueue true
      
      testCase "setting options and reading them are inverses: KeepLastMessageInQueue false" <| fun () ->
        <@ fun (KeepLastMessageInQueue actual) -> KeepLastMessageInQueue actual @>
        |> testInverses <| KeepLastMessageInQueue false

      testCase "setting options and reading them are inverses: LingerDelay" <| fun () ->
        <@ fun (LingerDelay actual) -> LingerDelay actual @>
        |> testInverses <| LingerDelay 123<ms>

      testCase "setting options and reading them are inverses: MaxMessageSize" <| fun () ->
        <@ fun (MaxMessageSize actual) -> MaxMessageSize actual @>
        |> testInverses <| MaxMessageSize 123L<B>

      testCase "setting options and reading them are inverses: MaxReconnectInterval" <| fun () ->
        <@ fun (MaxReconnectInterval actual) -> MaxReconnectInterval actual @>
        |> testInverses <| MaxReconnectInterval 123<ms>

      testCase "setting options and reading them are inverses: MulticastHops" <| fun () ->
        <@ fun (MulticastHops actual) -> MulticastHops actual @>
        |> testInverses <| MulticastHops 123<NetworkHop>

      testCase "setting options does not throw: ProbeRouter true" <| fun () ->
        ProbeRouter true
        |> testSetter ZMQ.REQ

      testCase "setting options does not throw: ProbeRouter false" <| fun () ->
        ProbeRouter false
        |> testSetter ZMQ.REQ
      
      testCase "setting options and reading them are inverses: ReceiveQueue" <| fun () ->
        <@ fun (ReceiveQueue actual) -> ReceiveQueue actual @>
        |> testInverses <| ReceiveQueue 123

      testCase "setting options and reading them are inverses: ReceiveTimeout" <| fun () ->
        <@ fun (ReceiveTimeout actual) -> ReceiveTimeout actual @>
        |> testInverses <| ReceiveTimeout 123<ms>
      
      testCase "setting options and reading them are inverses: ReconnectDelay" <| fun () ->
        <@ fun (ReconnectDelay actual) -> ReconnectDelay actual @>
        |> testInverses <| ReconnectDelay 123<ms>

      testCase "setting options does not throw: RelaxStrictAlternation true" <| fun () ->
        RelaxStrictAlternation true
        |> testSetter ZMQ.REQ

      testCase "setting options does not throw: RelaxStrictAlternation false" <| fun () ->
        RelaxStrictAlternation false
        |> testSetter ZMQ.REQ

      testCase "setting options does not throw: RequestCorrelation true" <| fun () ->
        RequestCorrelation true
        |> testSetter ZMQ.REQ

      testCase "setting options does not throw: RequestCorrelation false" <| fun () ->
        RequestCorrelation false
        |> testSetter ZMQ.REQ

      testCase "setting options does not throw: ResendDuplicateMessages true" <| fun () ->
        ResendDuplicateMessages true
        |> testSetter ZMQ.XPUB

      testCase "setting options does not throw: ResendDuplicateMessages false" <| fun () ->
        ResendDuplicateMessages false
        |> testSetter ZMQ.XPUB 
     
      testCase "setting options does not throw: RouterMandatory true" <| fun () ->
        RouterMandatory true
        |> testSetter ZMQ.ROUTER

      testCase "setting options does not throw: RouterMandatory false" <| fun () ->
        RouterMandatory false
        |> testSetter ZMQ.ROUTER

      testCase "setting options does not throw: RouterHandover false" <| fun () ->
        RouterHandover false
        |> testSetter ZMQ.ROUTER

      testCase "setting options does not throw: RouterHandover true" <| fun () ->
        RouterHandover true
        |> testSetter ZMQ.ROUTER

      testCase "setting options and reading them are inverses: SendQueue" <| fun () ->
        <@ fun (SendQueue actual) -> SendQueue actual @>
        |> testInverses <| SendQueue 123

      testCase "setting options and reading them are inverses: SendTimeout" <| fun () ->
        <@ fun (SendTimeout actual) -> SendTimeout actual @>
        |> testInverses <| SendTimeout 123<ms>
      
      testCase "setting options and reading them are inverses: TcpKeepalive None" <| fun () ->
        <@ fun (TcpKeepalive actual) -> TcpKeepalive actual @>
        |> testInverses <| TcpKeepalive None

      testCase "setting options and reading them are inverses: TcpKeepalive (Some false)" <| fun () ->
        <@ fun (TcpKeepalive actual) -> TcpKeepalive actual @>
        |> testInverses <| TcpKeepalive (Some false)

      testCase "setting options and reading them are inverses: TcpKeepalive (Some true)" <| fun () ->
        <@ fun (TcpKeepalive actual) -> TcpKeepalive actual @>
        |> testInverses <| TcpKeepalive (Some true)
      
      testCase "setting options and reading them are inverses: TcpKeepaliveCount" <| fun () ->
        <@ fun (TcpKeepaliveCount actual) -> TcpKeepaliveCount actual @>
        |> testInverses <| TcpKeepaliveCount 123

      testCase "setting options and reading them are inverses: TcpKeepaliveIdle" <| fun () ->
        <@ fun (TcpKeepaliveIdle actual) -> TcpKeepaliveIdle actual @>
        |> testInverses <| TcpKeepaliveIdle 123<s>

      testCase "setting options and reading them are inverses: TcpKeepaliveInterval" <| fun () ->
        <@ fun (TcpKeepaliveInterval actual) -> TcpKeepaliveInterval actual @>
        |> testInverses <| TcpKeepaliveInterval 123<s>

      testCase "setting options and reading them are inverses: HandshakeInterval" <| fun () ->
        <@ fun (HandshakeInterval actual) -> HandshakeInterval actual @>
        |> testInverses <| HandshakeInterval 123<ms>
    ]
    
  let parseSecurity = 
    <@ function
       | PlainServer -> PlainServer
       | PlainClient (unm, pwd) -> PlainClient(unm, pwd)
       | GssapiServer (principal) -> GssapiServer (principal)
       | GssapiServerUnencripted (principal) -> GssapiServerUnencripted (principal)
       | GssapiClient (principal, servicePrincipal) -> GssapiClient (principal, servicePrincipal)
       | GssapiClientUnencripted (principal, servicePrincipal) -> GssapiClientUnencripted (principal, servicePrincipal)
       | CurveServer secretKey -> CurveServer secretKey
       | CurveClient (publicKey, secretKey, serverKey) -> CurveClient (publicKey, secretKey, serverKey)
       | NullSecurity -> NullSecurity
       | _ -> failwith "unknown security mechanism" @>  

  [<Tests>]
  let security =
    testList "security" [
      testCase "getting the security mechanism does not throw" <| fun () ->
        parseSecurity
        |> testGetter NullSecurity  

      testCase "setting options and reading them are inverses: NullSecurity" <| fun () ->
        parseSecurity
        |> testInverses <| NullSecurity
        
      testCase "setting options and reading them are inverses: PlainServer" <| fun () ->
        parseSecurity
        |> testInverses <| PlainServer    

      testCase "setting options and reading them are inverses: PlainClient" <| fun () ->
        parseSecurity
        |> testInverses <| PlainClient ("myUserName", "supersecurepassword")
    ]

  /// Well-known keys from the ZMQ documentation.
  module private CurveKeys =
    let clientPublicKey = Z85.decode "Yne@$w-vo<fVvi]a<NY6T1ed:M$fCG*[IaLV{hID"
    let clientSecretKey = Z85.decode "D:)Q[IlAW!ahhC2ac:9*A}h:p?([4%wOTJ%JR%cs"

    let serverPublicKey = Z85.decode "rq:rM>}U?@Lns47E1%kR.o@n%FcmmsL/@{H8]yf7"
    let serverSecretKey = Z85.decode "JTKVSB%%)wK0E.X)V>+}o?pNmC{O&4W4b!Ni{Lh6"

  [<Tests>]
  let security2 =
    testList "security" [
      testCase "setting options and reading them are inverses: CurveServer" <| fun () ->   
        assertHas ZMQ.CAP_CURVE
        parseSecurity
        |> testInverses <| CurveServer CurveKeys.serverSecretKey

      testCase "setting options and reading them are inverses: CurveClient" <| fun () ->   
        assertHas ZMQ.CAP_CURVE
        parseSecurity
        |> testInverses <| CurveClient(CurveKeys.clientPublicKey, CurveKeys.clientSecretKey, CurveKeys.serverPublicKey)

      testCase "getting options does not throw: LastEndpointAddress" <| fun () ->
        <@ fun (LastEndpointAddress address) -> address @>
        |> testGetter ""

      testCase "setting options and reading them are inverses: TypeOfService" <| fun () ->
        <@ fun (TypeOfService tos) -> TypeOfService tos @>
        |> testInverses <| TypeOfService 123

      testCase "setting options does not throw: ConnectPeerId" <| fun () ->
        ConnectPeerId "otherpeer"B
        |> testSetter ZMQ.ROUTER

      testCase "setting options and reading them are inverses: SocksProxy" <| fun () ->
        <@ fun (SocksProxy proxy) -> SocksProxy proxy @>
        |> testInverses <| SocksProxy "1.2.3.4:5678"

      testCase "setting options does not throw: DoNoSilentlyDropMessages true" <| fun () ->
        DoNoSilentlyDropMessages true
        |> testSetter ZMQ.PUB

      testCase "setting options does not throw: DoNoSilentlyDropMessages false" <| fun () ->
        DoNoSilentlyDropMessages false
        |> testSetter ZMQ.PUB

      testCase "setting options and reading them are inverses: GssapiServer" <| fun () ->
        assertHas ZMQ.CAP_GSSAPI
        parseSecurity
        |> testInverses <| GssapiServer "principal"

      testCase "setting options and reading them are inverses: GssapiServerUnencrypted" <| fun () ->
        assertHas ZMQ.CAP_GSSAPI
        parseSecurity
        |> testInverses <| GssapiServerUnencripted "principal"

      testCase "setting options and reading them are inverses: GssapiClient" <| fun () ->
        assertHas ZMQ.CAP_GSSAPI
        parseSecurity
        |> testInverses <| GssapiClient ("principal", "servicePrincipal")

      testCase "setting options and reading them are inverses: GssapiClientUnencrypted" <| fun () ->
        assertHas ZMQ.CAP_GSSAPI
        parseSecurity
        |> testInverses <| GssapiClientUnencripted ("principal", "servicePrincipal")
    ]