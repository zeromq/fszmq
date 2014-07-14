(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
Weather Update Server
====================

Binds PUB socket to tcp://*:5556

Publishes random weather updates
*)

#r "fszmq.dll"
open fszmq
open System

// helper to convert strings to frames
let encode = string >> System.Text.Encoding.ASCII.GetBytes

// initialize random number generator
let rand = Random DateTime.Now.Millisecond
  
let main () = 
  // prepare our context and publisher
  use context   = new Context ()
  use publisher = Context.pub context
  Socket.bind publisher "tcp://*:5556"
  //Socket.bind publisher "icp://weather.ipc"
  //NOTE: IPC transport is not currently supported on Microsoft Windows

  while true do
    // get values that will fool the boss
    let zipcode     = rand.Next 100000
    let temperature = (rand.Next 215) - 80
    let relhumidity = (rand.Next 50) + 10

    // send message to all subscribers
    let update = sprintf "%05d %d %d" zipcode temperature relhumidity
    Socket.send publisher (encode update)

  0 // return code
  
(*** hide ***) 
main ()
