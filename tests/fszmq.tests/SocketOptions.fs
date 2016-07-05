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

  
  //TODO: for each ZMQ socket option that supports it
  //        test that "setting via SocketOption case"
  //        and "getting via active pattern" are inverses
  