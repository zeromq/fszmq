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
#I "../../packages/ExtCore.0.8.36/lib/net40"
#I "../../packages/FsCheck.0.9.2.0/lib/net40-client"
#I "../../packages/FsUnit.1.2.1.0/Lib/Net40"
#I "../../packages/NUnit.2.6.3/lib"
#I "./bin/Debug"
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/bin/Debug"
printfn "CurrentDirectory = %s" Environment.CurrentDirectory
#r "ExtCore.dll"
#r "FsCheck.dll"
#r "FsUnit.NUnit.dll"
#r "fszmq.dll"
#r "nunit.framework.dll"
#else
namespace fszmq.tests
#endif

open ExtCore
open FsCheck
open FsUnit
open fszmq
open NUnit.Framework

[<AutoOpen;TestFixture>]
module UnitTest = 

  [<Test;Category("Miscellany")>]
  let ``version should be 4.0.1``() =
    let vsn = ZMQ.version
    printfn "%A" vsn
    vsn |> should equal (Version(4,0,4))

(* ZCURVE & Z85 Tests *)
  let [<Literal>] HAUSNUMERO = 156384712
  
  let [<Literal>] POSIX_ENOTSUP = 129
  let [<Literal>] WINXX_ENOTSUP = HAUSNUMERO ||| 1
  
  let BINARY = [| 0x86uy; 0x4Fuy; 0xD2uy; 0x6Fuy; 0xB5uy; 0x59uy; 0xF7uy; 0x5Buy |]
  let STRING = "HelloWorld"

  let inline isIn items v = Seq.exists ((=) v) items

  [<Test;Category("ZCURVE")>]
  let ``keypair generation requires sodium`` () =
    let codes = [ POSIX_ENOTSUP; WINXX_ENOTSUP; ]
    let error = Assert.Throws<ZMQError> (fun () -> Curve.curveKeyPair() |> ignore)
    error.Message |> should equal "Not supported"
    codes |> should contain error.ErrorNumber

  //TODO: write passing tests, once you figure out libsodium installation

  [<Test;Category("Z85")>]
  let ``can encode (binary-to-string)`` () =
    let encoded = Z85.encode(BINARY)
    encoded |> should equal STRING
  
  //TODO: add more tests (mostly failing) for Z85.encode

  [<Test;Category("Z85")>]
  let ``can decode (string-to-binary)`` () =
    let decoded = Z85.decode(STRING)
    decoded |> should equal BINARY
      
  //TODO: add more tests (mostly failing) for Z85.decode
