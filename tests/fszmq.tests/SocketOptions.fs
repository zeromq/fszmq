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

  [<Test>]
  let ``setting options and reading them are inverses: Affinity`` () =
    fun socket -> <@ let (Affinity actual) = socket in Affinity actual @>
    |> testInverses <| Affinity 0x123UL
  
  [<Test>]
  let ``setting options and reading them are inverses: Identity`` () =
    fun socket -> <@ let (Identity actual) = socket in Identity actual @>
    |> testInverses <| Identity "My Identity"B

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
  let ``setting options and reading them are inverses: MoreMessageFramesAvailable`` () =
    fun socket -> <@ let (MoreMessageFramesAvailable actual) = socket in MoreMessageFramesAvailable actual @>
    |> testInverses <| MoreMessageFramesAvailable true

  [<Test>]
  let ``setting options and reading them are inverses: Events`` () =
    fun socket -> <@ let (Events actual) = socket in Events actual @>
    |> testInverses <| Events 123

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


// TODO: test Subscribe
// TODO: test Unsubscribe
// TODO: test plain security (PlainServer true; PlainServer false; PlainClient (unm, pwd))
// TODO: test curve security (CurveServer (true, secretKey); CurveServer (false, _); CurveClient(publicKey, secretKey,serverKey) 
// TODO: refactor CurveServer (bool * byte[]) => CurveServerEnabled byte[]; CurveServerDisabled
  