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
Request-Reply Broker
====================

Simple broker for connecting client requests to server replies.
*)

#r "fszmq.dll"
open fszmq
open fszmq.Polling

// helper for managing resources
let dispose (o : System.IDisposable) = if o <> null then o.Dispose()

let main () = 
  // prepare our context and sockets
  use context   = new Context ()
  use frontend  = Context.router context
  use backend   = Context.dealer context
  Socket.bind frontend  "tcp://*:5559"
  Socket.bind backend   "tcp://*:5560"

  let transfer inbound outbound =
    let rec loop () =
      // process all parts of the message
      let message = Message.recv inbound
      if Message.hasMore message
        then  Message.sendMore outbound message
              dispose message
              loop ()
        else  // last message part
              Message.send outbound message
              dispose message
    loop ()

  // initialize poll set
  let items =
    [ frontend |> pollIn (fun _ -> transfer frontend backend )
      backend  |> pollIn (fun _ -> transfer backend  frontend) ]

  // switch messages between sockets
  while items |> pollForever do ((* nothing *))
    
  0 // return code

(*** hide ***)    
main ()
