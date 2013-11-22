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
namespace fszmq.tests

open ExtCore
open FsUnit
open fszmq
open NUnit.Framework
open System

module Miscellany =
    
  [<Test>]
  let ``scratch`` () =
    printfn "This is a test." //TODO: is there a more idiomatically NUnit way to do logging?
    true |> should equal true

module Z85 =

  [<Test>]
  let ``keypair generation requires sodium`` () =
    
    let HAUSNUMERO = 156384712
  
    let POSIX_ENOTSUP = 129
    let WINXX_ENOTSUP = HAUSNUMERO ||| 1
    
    let err = Assert.Throws<ZMQError>(fun () -> Z85.curveKeyPair() |> ignore)
    [POSIX_ENOTSUP;WINXX_ENOTSUP;] |> should contain err.ErrorNumber 
    "Not supported" |> should equal err.Message

  //TODO: write passing tests, once you figure out libsodium installation

  [<Test>]
  let ``can encode (binary-to-string)`` () =
    let BINARY  = [| 0x86uy; 0x4Fuy; 0xD2uy; 0x6Fuy; 0xB5uy; 0x59uy; 0xF7uy; 0x5Buy |]
    let encoded = Z85.encode(BINARY)
    printfn "%s" encoded
    encoded |> should equal "HelloWorld"
  
  //TODO: add more tests (mostly failing) for Z85.encode

  [<Test>]
  let ``can decode (string-to-binary)`` () =
    let STRING  = "HelloWorld"
    let decoded = Z85.decode(STRING)
    printfn "%A" decoded
    decoded |> should equal [| 0x86uy; 0x4Fuy; 0xD2uy; 0x6Fuy; 0xB5uy; 0x59uy; 0xF7uy; 0x5Buy |]
  
  //TODO: add more tests (mostly failing) for Z85.decode
