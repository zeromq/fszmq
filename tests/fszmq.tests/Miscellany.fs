namespace fszmq.tests

open FsCheck
open fszmq
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module Miscellany =

  [<Test>]
  let ``libzmq version should be at least 4``() =
    test <@ match ZMQ.version with
            | Version(major,minor,build) -> major = 4 && minor >= 0 && build >= 0 
            | Version.Unknown            -> Unknown <> Unknown @>

  [<Test>]
  let ``recv throws TimeoutException if RCVTIMEO expires`` () =
    let testFn () =
      use ctx = new Context ()
      let sck = Context.pair ctx
      Socket.setOption sck (ZMQ.RCVTIMEO,10)
      Socket.bind sck "inproc://dummy"
      sck
      |> Socket.recv
      |> printfn "msg: %A"

    raises<TimeoutException> <@ testFn () @>

[<TestFixture>]
module Z85 =

  [<Test>]
  let ``encode-then-decode preserves data`` () =
    let isValid = function
      | null
      | [||] -> false
      | data -> Array.length data % 4 = 0
    
    Check.QuickThrowOnFailure (fun data ->
      (isValid data) ==> lazy (data |> Z85.encode |> Z85.decode = data))
