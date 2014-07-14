(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
Interrupt Handling
====================

Shows how to handle manual interrupt (i.e. CTRL+C) in a console application.

_Note: similar (though not identical) techniques exist for 
other types of applications (e.g. desktop appplications, daemons, et cetera)._
*)
#r "fszmq.dll"
open fszmq
open fszmq.Polling
open System.Threading

// helper to convert between frames to strings
let decode = System.Text.Encoding.ASCII.GetString

let main () = 
  use context = new Context ()
  use replyer = Context.rep context
  Socket.bind replyer "tcp://*:5555"
  
  let interrupted = ref false
  System.Console.CancelKeyPress |> Event.add (fun e ->  interrupted := true
                                                        e.Cancel    <- true)
  
  while not !interrupted do
    Message.tryRecv replyer ZMQ.DONTWAIT
    |> Option.iter (fun msg ->  let message = decode <| Message.data msg
                                printfn "Received request: %s" message
                                // simulate work, by sleeping
                                Thread.Sleep 1000
                                // send reply back to client
                                Socket.send replyer "World"B)

  0 // return code

(*** hide ***)
main ()
