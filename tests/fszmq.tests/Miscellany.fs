namespace fszmq.tests

open System
open FsCheck
open Expecto
open Swensen.Unquote
open fszmq

module Miscellany =
  [<Tests>]
  let tests =
    testList "misc" [
      testCase "libzmq version should be at least 4" <| fun () ->
        test <@ match ZMQ.version with
                | Version(major,minor,build) -> major = 4 && minor >= 0 && build >= 0 
                | Version.Unknown            -> Unknown <> Unknown @>

      testCase "recv throws TimeoutException if RCVTIMEO expires" <| fun () ->
        let testFn () =
          use ctx = new Context ()
          let sck = Context.pair ctx
          Socket.setOption sck (ZMQ.RCVTIMEO,10)
          Socket.bind sck "inproc://dummy"
          sck
          |> Socket.recv
          |> printfn "msg: %A"

        raises<TimeoutException> <@ testFn () @>
    ]
     
module Z85 =

  [<Tests>]
  let tests =
    testCase "encode-then-decode preserves data" <| fun () ->
      let isValid = function
        | null
        | [||] -> false
        | data -> Array.length data % 4 = 0
      
      Check.QuickThrowOnFailure (fun data ->
        (isValid data) ==> lazy (data |> Z85.encode |> Z85.decode = data))


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

  [<Tests>]
  let tests =
    testCase "curveKeyPair generates 2 different evenly-divisible strings" <| fun () ->
      Arb.register<CurveKeyPairArb> () |> ignore

      match ZMQ.has ZMQ.CAP_CURVE with
      | Supported (_,true ) ->  Check.QuickThrowOnFailure CurveKeyPairProperties
      | Supported (_,false) ->  
        skiptest "CURVE not supported"
      | Unknown             -> 
        skiptest "Unable to determine CURVE support"