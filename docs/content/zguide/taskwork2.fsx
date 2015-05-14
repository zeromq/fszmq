(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
open System
open System.IO

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
Task Worker (design 2)
====================

Connects PULL socket to tcp://localhost:5557

Collects workloads from ventilator via that socket

Connects PUSH socket to tcp://localhost:5558

Sends results to sink via that socket

__Update:__

Adds PUB-SUB flow to receive and respond to kill signal
*)
#r "fszmq.dll"
open fszmq
open fszmq.Polling
open System.Threading

// helpers to convert between strings and frames
let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString

let main () = 
  use context = new Context ()
  
  // socket to receive messages on
  use receiver = Context.pull context
  Socket.connect receiver "tcp://localhost:5557"

  // socket to send messages to
  use sender = Context.push context
  Socket.connect sender "tcp://localhost:5558"

  // socket for control input
  use controller = Context.sub context
  Socket.connect controller "tcp://localhost:5559"
  Socket.subscribe controller [""B] // subscribe to all message topics

  // process messages from either socket
  let again = ref true
  let items =
    [ receiver    |> pollIn (fun _ -> let msg = receiver |> Socket.recv |> decode
                                      // show progress
                                      printf "%s." msg
                                      // do the work
                                      Thread.Sleep (int msg)
                                      // send results to sink
                                      Socket.send sender ""B)
      // Any waiting controller command acts as 'KILL'
      controller  |> pollIn (fun _ -> again := false (* exit loop *)) ]

  while !again do 
    items |> pollForever |> ignore
  
  0 // return code
 
(*** hide ***)
main ()
