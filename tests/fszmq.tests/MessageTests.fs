(* ------------------------------------------------------------------------
This file is part of fszmq.

fszmq is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published 
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

fszmq is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with fszmq. If not, see <http://www.gnu.org/licenses/>.

Copyright (c) 2011-2013 Paulmichael Blasucci
------------------------------------------------------------------------ *)
#if INTERACTIVE
open System
#I "../../packages/FsUnit.1.2.1.0/Lib/Net40"
#I "../../packages/NUnit.2.6.3/lib"
#I "./bin/Debug"
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/bin/Debug"
printfn "CurrentDirectory = %s" Environment.CurrentDirectory
#r "FsUnit.NUnit.dll"
#r "fszmq.dll"
#r "nunit.framework.dll"
#else
namespace fszmq.tests
#endif

open FsUnit
open fszmq
open NUnit.Framework

[<AutoOpen;TestFixture>]
module MessageTest = 
  
  open fszmq.Message

  [<Test;Category("Message Manipulation")>]
  let ``clone returns a distinct instance with identical content`` () =
    use msg1 = new Message("test"B)
    use msg2 = Message.clone msg1
    msg2 |> should not' (equal msg1)
    (size msg2) |> should equal (size msg1)
    (data msg2) |> should equal (data msg1)

  [<Test;Category("Message Manipulation")>]
  let ``both message should have the same content after copying`` () =
    use msg1 = new Message("test"B)
    use msg2 = new Message("sample"B)
    (data msg2) |> should not' (equal (data msg1))
    Message.copy msg1 msg2
    (data msg2) |> should equal (data msg1)

  [<Test;Category("Message Manipulation")>]
  let ``copy requires two distinct message instances`` () =
    let error = Assert.Throws<ZMQError> (fun () -> use msg = new Message()
                                                   Message.copy msg msg)
    error.Message |> should equal "Invalid argument"

  [<Test;Category("Message Manipulation")>]
  let ``after moving, target content should equal original content`` () =
    use source = new Message("test"B)
    use target = new Message("sample"B)
    let srcData = data source
    (data target) |> should not' (equal srcData)
    Message.move source target
    (data target) |> should equal srcData

  [<Test;Category("Message Manipulation")>]
  let ``after moving, source message content should be empty`` () =
    use source = new Message("test"B)
    use target = new Message()
    (data target) |> should not' (equal (data source))
    Message.move source target
    (size source) |> should equal 0
    (data source) |> should not' (equal (data target))

  [<Test;Category("Message Manipulation")>]
  let ``move requires two distinct message instances`` () =
    let error = Assert.Throws<ZMQError> (fun () -> use msg = new Message()
                                                   Message.move msg msg)
    error.Message |> should equal "Invalid argument"
