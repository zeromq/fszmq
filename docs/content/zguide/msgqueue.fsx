(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
Message Queue Broker
====================

Simple message queuing broker, same as request-reply broker but using device.
*)

#r "fszmq.dll"
open fszmq
open fszmq.Proxying

let main () = 
  use context = new Context ()

  // socket facing clients
  use frontend = Context.router context
  Socket.bind frontend "tcp://*:5559"

  // socket facing services
  use backend = Context.router context
  Socket.bind backend "tcp://*:5560"

  // start the proxy
  proxy frontend backend None

  0 // return code

(*** hide ***)    
main ()
