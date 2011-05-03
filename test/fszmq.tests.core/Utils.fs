(*-------------------------------------------------------------------------
                                                                           
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
module fszmq.tests.core.Utils

open fszmq
open fszmq.Context
open fszmq.Socket
open fszmq.Polling
open System

// create a pair of sockets connected to one another
let createBoundPair context role1 role2 address =
  let sock1,sock2 = newSocket context role1,
                    newSocket context role2
  address |> bind sock1
  address |> connect sock2
  sock1,sock2

// send a message from one socket in the pair to the other and back
let pingPong sock1 sock2 ping =
  // send ping out
  ping |>> sock1

  // get pong from connected socket
  let pong = recv sock2

  // send message via s2, so state is clean in case of req-rep
  pong |>> sock2

  // return received data
  pong

let basicTests role1 role2 address =
  use ctx = new Context(1)
  let sock1,sock2 = createBoundPair ctx role1 role2 address
  try
    // first test simple ping-pong
    let expected = "XXX"B
    let returned = expected |> pingPong sock1 sock2
    assert (expected = returned)

    // adjust socket state so that poll shows only 1 pending message
    let msg = recv sock1

    // now poll is used to signal that a message is ready to read
    let returned = ref ""B
    let pollItems = seq { 
      yield Poll(ZMQ.POLLIN,sock1,ignore)
      yield Poll(ZMQ.POLLIN,sock2,fun s -> returned := recv s) }

    sock1 <<| expected
    let rc = pollItems |> poll -1L
    assert rc
    assert (expected = !returned)
  
  // delete sockets
  finally
    (sock1 :> IDisposable).Dispose()
    (sock2 :> IDisposable).Dispose()
