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
Hello World client
====================

Connects REQ socket to tcp://localhost:5555

Sends "Hello" to server, expects "World" back
*)

#r @"fszmq.dll"
open fszmq

let main () = 
  printfn "Connecting to hello world server..."
  use context = new Context ()
  use requester = Context.req context 
  Socket.connect requester "tcp://localhost:5555"

  for request_nbr in 0 .. 9 do
    printfn "Sending Hello %d..." request_nbr
    Socket.send requester "Hello"B 
    let _buffer = Socket.recv requester
    printfn "Received World %d" request_nbr
    
  0 // return code

(*** hide ***)
main ()
