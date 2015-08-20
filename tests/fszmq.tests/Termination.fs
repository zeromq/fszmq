namespace fszmq.tests

open FsCheck
open fszmq
open fszmq.Polling
open NUnit.Framework
open System
open System.Threading

[<TestFixture>]
module EchoTest =

  let serverThread o =
    let token,context,address = unbox<CancellationToken * Context * string> o

    if not token.IsCancellationRequested then
      let echo = Context.router context //NOTE: NOT disposed at end of scope
      address |> Socket.bind echo

      let sockets = [ echo |> pollIn (Socket.recvAll >> Socket.sendAll echo)]
      while not token.IsCancellationRequested do
        let didFire = sockets |> poll 100L
        if not didFire then printfn "not fired"

  let clientThread o =
    let token,context,address = unbox<CancellationToken * Context * string> o

    if not token.IsCancellationRequested then
      use echo = Context.dealer context //NOTE: disposed at end of scope
      address |> Socket.connect echo
      let sockets = [ echo |> pollIn (Socket.recvAll >> ignore)]
      while not token.IsCancellationRequested do
        "ping"B |> Socket.send echo 
        let didFire = sockets |> pollForever
        if not didFire then printfn "no reply"

  let main (delay:int) =
    use source  = new CancellationTokenSource (delay)
    let context = new Context ()
    let address = "tcp://127.0.0.1:1979"
    let state   = (source.Token,context,address)

    let echoServer = Thread (ParameterizedThreadStart serverThread)
    let echoClient = Thread (ParameterizedThreadStart clientThread)
    echoServer.Start state
    echoClient.Start state 

    Console.WriteLine (sprintf "(%i) running" delay)
    echoClient.Join ()
    echoServer.Join ()
    
    (context :> IDisposable).Dispose()
    0 // OK

  [<Test>]
  let ``everything should shutdown cleanly`` () = 
    Check.QuickThrowOnFailure (fun (PositiveInt delay) -> main delay = 0)
    