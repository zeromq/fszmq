namespace fszmq.tests

open FsCheck
open fszmq
open fszmq.Message
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module Message =

  [<Test>]
  let ``copy requires two distinct message instances`` () =
    raises<ZMQError> <@ use msg = new Message() in Message.copy msg msg @>

  [<Test>]
  let ``move requires two distinct message instances`` () =
    raises<ZMQError> <@ use msg = new Message() in Message.move msg msg @>

  [<Test>]
  let ``clone returns a new instance with the same data`` () =
    Check.QuickThrowOnFailure (fun data ->
      use msg1 = new Message (data)
      use msg2 = Message.clone msg1

      msg1 <> msg2 && isMatch msg1 msg2)

  [<Test>]
  let ``both message should have the same content after copying`` () =
    Check.QuickThrowOnFailure (fun data1 data2 ->
      (data1 <> data2) ==> 
        lazy (use msg1 = new Message (data1)
              use msg2 = new Message (data2)

              let precheck =  msg1 <> msg2 && not (isMatch msg1 msg2)
              Message.copy msg1 msg2
              let postcheck = msg1 <> msg2 && isMatch msg1 msg2

              precheck && postcheck))

  [<Test>]
  let ``after moving, target content should equal original content`` () =
    Check.QuickThrowOnFailure (fun data1 data2 ->
      (data1 <> data2) ==> 
        lazy (use msg1 = new Message (data1)
              use msg2 = new Message (data2)

              let precheck =  msg1 <> msg2 && not (isMatch msg1 msg2)
              Message.move msg1 msg2 
              let postcheck = msg1 <> msg2 
                              && data1 = Message.data msg2 
                              && Message.data msg1 = [||]

              precheck && postcheck))

  open Options
  open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

  let test socket =
   configureSocket socket [Rate 12<kb/s>]
   let (Rate rate) = socket
   rate
