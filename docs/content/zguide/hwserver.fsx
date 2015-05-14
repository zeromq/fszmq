(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
open System

let workingFolder =
  match Environment.OSVersion.Platform with
  | PlatformID.Unix 
  | PlatformID.MacOSX ->  "" //NOTE: on WIN we need different paths per architecture
  | _                 ->  if Environment.Is64BitProcess then "x64" else "x86"
  |> sprintf "../../bin/%s"       
//NOTE: fszmq.dll needs to "see" libzmq.dll...
//      force that by running in the deployment staging folder
Environment.CurrentDirectory <- workingFolder 

(**
Hello World server
====================

Binds REP socket to tcp://*:5555

Expects "Hello" from client, replies with "World"
*)

#r @"fszmq.dll"
open fszmq
open fszmq.Socket
open System.Threading

let main () = 
  // socket to talk to clients
  use context = new Context ()
  use responder = Context.rep context
  Socket.bind responder "tcp://*:5555"

  while true do
    // wait for next request from client
    let _buffer = Socket.recv responder
    printfn "Received Hello"
    
    // do some work
    Thread.Sleep 1000 // msecs

    // send reply back to client
    Socket.send responder "World"B
 
  0 // return code

(*** hide ***)
main ()
