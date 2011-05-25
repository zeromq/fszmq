module fszmq.devices.tests.echo_server

open fszmq
open fszmq.Context
open fszmq.Socket
open fszmq.devices

[<AutoOpen>]
module private Utilities =
    
  (* program return codes *)
  let [<Literal>] OKAY = 0
  let [<Literal>] FAIL = 3

  (* I/O helpers *)
  let scanln = System.Console.ReadLine
  let encode = string >> System.Text.Encoding.ASCII.GetBytes
  let decode = System.Text.Encoding.ASCII.GetString

  let echo _ message =
    // this is the actual "work" performed by the server on each request
    System.Threading.Thread.Sleep 1000
    message

[<EntryPoint>]
let main args = 
  let result = ref OKAY
  try
    // initialize context ...
    use ctx = new Context 1
    // initialize server ...
    use srv = new Server (ctx,echo,args.[0])
    srv.Error.Add (fun x -> printfn "WARN: %s" x.Message)
    // we're off and running!
    srv.Start ()
    printf "press <return> to stop server..."
    scanln () |> ignore
  with
  | x ->  result := FAIL
          printfn "FAIL: %s" x.Message
            
  printf "press <return> to exit..."
  scanln () |> ignore
  exit !result
