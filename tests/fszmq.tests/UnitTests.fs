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

open System

#if INTERACTIVE
#I "./bin/Debug"
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/bin/Debug"
printfn "CurrentDirectory = %s" Environment.CurrentDirectory
#r "fszmq.dll"
#endif

module Z85 =

  open fszmq

  let [<Literal>] HAUSNUMERO = 156384712
  
  let [<Literal>] POSIX_ENOTSUP = 129
  let [<Literal>] WINXX_ENOTSUP = HAUSNUMERO ||| 1
  
  let BINARY = [| 0x86uy; 0x4Fuy; 0xD2uy; 0x6Fuy; 0xB5uy; 0x59uy; 0xF7uy; 0x5Buy |]
  let STRING = "HelloWorld"

  let inline isIn items v = Seq.exists ((=) v) items

  let ``keypair generation requires sodium`` () =
    let errorCodes = [ POSIX_ENOTSUP; WINXX_ENOTSUP; ]
    try
      Z85.curveKeyPair() |> ignore
    with
    | :? ZMQError as x -> 
            assert (x.ErrorNumber |> isIn errorCodes)
            assert (x.Message = "Not supported"     )
            printfn "TEST: keypair generation requires sodium"

  //TODO: write passing tests, once you figure out libsodium installation

  let ``can encode (binary-to-string)`` () =
    let encoded = Z85.encode(BINARY)
    assert (encoded = STRING)
    printfn "TEST: %s = %s" STRING encoded
  
  //TODO: add more tests (mostly failing) for Z85.encode

  let ``can decode (string-to-binary)`` () =
    let decoded = Z85.decode(STRING)
    assert (decoded = BINARY)
    printfn "TEST: %A = %A" BINARY decoded
  
  //TODO: add more tests (mostly failing) for Z85.decode

module Program =
  
  let [<Literal>] OKAY = 0

  [<EntryPoint>]
  let main _ =
    printfn "TEST: %A" fszmq.ZMQ.version

    Z85.``keypair generation requires sodium``()
    Z85.``can encode (binary-to-string)``()
    Z85.``can decode (string-to-binary)``()
     
    OKAY
