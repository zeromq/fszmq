module fszmq.tests.Message

open FsCheck
open fszmq
open fszmq.Message
open Expecto
open Swensen.Unquote

[<Tests>]
let tests  =
  testList "message" [
    testCase "copy requires two distinct message instances" <| fun () ->
      raises<ZMQError> <@ use msg = new Message() in Message.copy msg msg @>

    testCase "move requires two distinct message instances" <| fun () ->
      raises<ZMQError> <@ use msg = new Message() in Message.move msg msg @>

    testCase "clone returns a new instance with the same data" <| fun () ->
      Check.QuickThrowOnFailure (fun data ->
        use msg1 = new Message (data)
        use msg2 = Message.clone msg1

        msg1 <> msg2 && isMatch msg1 msg2)

    testCase "both message should have the same content after copying" <| fun () ->
      Check.QuickThrowOnFailure (fun data1 data2 ->
        (data1 <> data2) ==> 
          lazy (use msg1 = new Message (data1)
                use msg2 = new Message (data2)

                let precheck =  msg1 <> msg2 && not (isMatch msg1 msg2)
                Message.copy msg1 msg2
                let postcheck = msg1 <> msg2 && isMatch msg1 msg2

                precheck && postcheck))

    testCase "after moving, target content should equal original content" <| fun () ->
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
  ]