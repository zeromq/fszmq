#I @"C:\working\projects\ThirdParty\fs-zmq\lib\zeromq-2.1.6"
// NOTE:  changing the current directory is the easiest way to ensure
//        the (native) libzmq.dll is available for use in the REPL 
System.Environment.CurrentDirectory <- 
  @"C:\working\projects\ThirdParty\fs-zmq\lib\zeromq-2.1.6"

#load "Native.fs"
open fszmq
#load "Core.fs"
open fszmq
#load "Memory.fs"
open fszmq.NativeMemory
#load "Constants.fs"
open fszmq
#load "Socket.fs"
open fszmq.Socket
#load "Context.fs"
open fszmq.Context
#load "Polling.fs"
open fszmq
open fszmq.Polling

#time "on"

exception BadReply of string

let [<Literal>] LOCAL  = "inproc://repl_tester"
let [<Literal>] REMOTE = "tcp://127.0.0.01:1979"

let scanln = System.Console.ReadLine
let encode (s:string) = System.Text.Encoding.ASCII.GetBytes(s)
let decode = System.Text.Encoding.ASCII.GetString

let cancel = Async.DefaultCancellationToken
let echoServer rep = 
  let rec serve () = async {
    if cancel.IsCancellationRequested 
      then  return ()
      else  let msg = rep |> recv
            printfn "echo: %s" (msg |> decode)
            msg |>> rep 
            return! serve () }
  serve ()

let echo () =
  printfn "enter a message and press <return>."
  printfn "press <return> without any message to exit."
  use ctx = new Context(1)
  use rep = ctx |> rep
  use req = ctx |> req
  LOCAL |> bind     rep
  LOCAL |> connect  req
  Async.Start(echoServer rep)
  let rec loop = function
    | null | "" ->  Async.CancelDefaultToken()
                    printfn "done!"
    | otherwise ->  otherwise |> encode |>> req
                    let msg = req |> recv |> decode
                    if otherwise <> msg then raise (BadReply msg)
                    loop (scanln ())
  loop (scanln ())

// TODO: figure out why 'scanln' is killing FSI's printing capabilities
// TODO: figure out why the REPL session is lost after echo exits

