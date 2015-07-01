(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"
#load "../docs.fs"
open docs
PATH.hijack ()

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
    use message = new Message ()
    let rec loop () =
      // process all parts of the message
      Message.recv message inbound
      if Message.hasMore message
        then  Message.sendMore message outbound
              loop ()
        else  // last message part
              Message.send message outbound
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
PATH.release ()
