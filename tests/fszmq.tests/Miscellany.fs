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


[<TestFixture>] 
module Curve =
  type CurveKeyPair = CurveKeyPair of publicKey:string * privateKey:string

  type CurveKeyPairArb =
    static member CurveKeyPair = 
      Arb.fromGen (gen {  let publicKey,privateKey = Curve.curveKeyPair ()
                          return CurveKeyPair (publicKey,privateKey)  })

  let CurveKeyPairProperties (CurveKeyPair (publicKey,privateKey)) =
         publicKey.Length  % 5 = 0 
      && privateKey.Length % 5 = 0 
      && publicKey <> privateKey

  [<Test>]
  let ``curveKeyPair generates 2 different evenly-divisible strings`` () =
    Arb.register<CurveKeyPairArb> () |> ignore

    match ZMQ.has ZMQ.CAP_CURVE with
    | Supported (_,true ) ->  Check.QuickThrowOnFailure CurveKeyPairProperties
    | Supported (_,false) ->  Assert.Ignore "CURVE not supported"
    | Unknown             ->  Assert.Inconclusive "Unable to determine CURVE support"
