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
module fszmq.perf.inproc.HelloWorld

open fszmq
open System
open System.Text
open System.Threading

let [<Literal>] HELLOWORLD_PIPE = "tcp://127.0.0.1:8989"

let client () =
  Thread.Sleep 10

  use context = new Context()
  use socket  = Context.req context
  
  Socket.connect socket HELLOWORLD_PIPE

  use msg = new Message (Encoding.UTF8.GetBytes "Hello")
  Message.send socket msg

  use reply = Message.recv socket
  Console.WriteLine (Encoding.UTF8.GetString <| Message.data reply)

let worker () =
  use context = new Context()
  use socket  = Context.rep context
  
  Socket.bind socket HELLOWORLD_PIPE
  
  use request = Message.recv socket
  Console.WriteLine (Encoding.UTF8.GetString <| Message.data request)

  use reply = new Message (Encoding.UTF8.GetBytes "World")
  Message.send socket reply

let benchmark = { new ITest with
                    member __.TestName = "Hello World" 
                    member __.RunTest () = 
                      let client = Thread (client)
                      let worker = Thread (worker)

                      worker.Start ()
                      client.Start ()

                      worker.Join () |> ignore
                      client.Join () |> ignore }
