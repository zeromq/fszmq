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
open fszmq
open System
open Xunit

module Miscellany =
    
  [<Fact>]
  let ``scratch`` () =
    printfn "This is a test." //TODO: is there a more idiomatically xUnit way to do logging?
    Assert.True(true)

module Z85 =

  [<Fact>]
  let ``keypair generation requires sodium`` () =
    
    let HAUSNUMERO = 156384712
  
    let POSIX_ENOTSUP = 129
    let WINXX_ENOTSUP = HAUSNUMERO ||| 1
    
    let err = Assert.Throws<ZMQError>(fun () -> Z85.curveKeyPair() |> ignore)
    Assert.True(Seq.exists ((=)err.ErrorNumber) [POSIX_ENOTSUP;WINXX_ENOTSUP])
    Assert.Equal<string>("Not supported",err.Message,StringComparer.InvariantCultureIgnoreCase)
    
  //TODO: write passing tests, once you figure out libsodium installation

  [<Fact>]
  let ``can encode (binary-to-string)`` () =
    let BINARY  = [| 0x86uy; 0x4Fuy; 0xD2uy; 0x6Fuy; 0xB5uy; 0x59uy; 0xF7uy; 0x5Buy |]
    let encoded = Z85.encode(BINARY)
    printfn "%s" encoded
    Assert.Equal<string>("HelloWorld",encoded)
  
  //TODO: add more tests (mostly failing) for Z85.encode

  [<Fact>]
  let ``can decode (string-to-binary)`` () =
    let STRING  = "HelloWorld"
    let decoded = Z85.decode(STRING)
    printfn "%A" decoded
    Assert.Equal<byte[]>([| 0x86uy; 0x4Fuy; 0xD2uy; 0x6Fuy; 0xB5uy; 0x59uy; 0xF7uy; 0x5Buy |],decoded)
  
  //TODO: add more tests (mostly failing) for Z85.decode
