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
