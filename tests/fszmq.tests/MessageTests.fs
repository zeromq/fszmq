namespace fszmq.tests

open fszmq
open NUnit.Framework

[<TestFixture>]
module MessageTest =

  open fszmq.Message

  [<Test;Category("Message Manipulation")>]
  let ``clone returns a new instance`` () =
    use msg1 = new Message("test"B)
    use msg2 = Message.clone msg1
    Assert.That (msg2,Is.Not.EqualTo msg1)
    Assert.That (size msg2,Is.EqualTo (size msg1))
    Assert.That (data msg2,Is.EqualTo (data msg1))

  [<Test;Category("Message Manipulation")>]
  let ``both message should have the same content after copying`` () =
    use msg1 = new Message("test"B)
    use msg2 = new Message("sample"B)
    Assert.That (data msg2,Is.Not.EqualTo (data msg1))
    Message.copy msg1 msg2
    Assert.That (data msg2,Is.EqualTo (data msg1))

  [<Test;Category("Message Manipulation")>]
  let ``copy requires two distinct message instances`` () =
    let error = Assert.Throws<ZMQError> (fun () -> use msg = new Message()
                                                   Message.copy msg msg)
    Assert.That (error.Message,Is.EqualTo "Invalid argument")

  [<Test;Category("Message Manipulation")>]
  let ``after moving, target content should equal original content`` () =
    use source = new Message("test"B)
    use target = new Message("sample"B)
    let srcData = data source
    Assert.That (data target,Is.Not.EqualTo srcData)
    Message.move source target
    Assert.That (data target,Is.EqualTo srcData)

  [<Test;Category("Message Manipulation")>]
  let ``after moving, source message content should be empty`` () =
    use source = new Message("test"B)
    use target = new Message()
    Assert.That (data target,Is.Not.EqualTo (data source))
    Message.move source target
    Assert.That (size source,Is.EqualTo 0)
    Assert.That (data source,Is.Not.EqualTo (data target))

  [<Test;Category("Message Manipulation")>]
  let ``move requires two distinct message instances`` () =
    let error = Assert.Throws<ZMQError> (fun () -> use msg = new Message()
                                                   Message.move msg msg)
    Assert.That (error.Message,Is.EqualTo "Invalid argument")
