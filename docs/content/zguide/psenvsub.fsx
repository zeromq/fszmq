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
Pub-Sub Message Evelopes
====================

Subscriber only wants messages whose topic frame is "B"
*)
#r "fszmq.dll"
open fszmq
open System.Text

let s_recv = Socket.recv >> Encoding.ASCII.GetString

let main () = 
  // prepare our context and subscriber
  use context     = new Context ()
  use subscriber  = Context.sub context
  Socket.connect subscriber "tcp://localhost:5563"
  Socket.subscribe subscriber [| "B"B |]

  while true do
    // read envelope with address
    let address = s_recv subscriber
    // read message contents
    let contents = s_recv subscriber
    printfn "[%s] %s" address contents
 
  0 // return code

(*** hide ***)
main ()
