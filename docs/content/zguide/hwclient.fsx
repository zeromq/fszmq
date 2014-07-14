(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

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
