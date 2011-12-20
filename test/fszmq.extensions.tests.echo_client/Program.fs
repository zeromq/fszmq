module fszmq.extensions.tests.echo_client

open fszmq
open fszmq.Context
open fszmq.Socket

[<AutoOpen>]
module private Utilities =

  (* timing functions *)
  open System.Runtime.InteropServices

  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern nativeint zmq_stopwatch_start()
  
  [<DllImport("libzmq",CallingConvention=CallingConvention.Cdecl)>]
  extern uint32 zmq_stopwatch_stop(nativeint watch)

  (* program return codes *)
  let [<Literal>] OKAY = 0
  let [<Literal>] FAIL = 3

  (* I/O helpers *)
  let scanln = System.Console.ReadLine
  let encode = string >> System.Text.Encoding.ASCII.GetBytes
  let decode = System.Text.Encoding.ASCII.GetString

  let prompt msg = 
    printf "%s " msg
    scanln ()

[<EntryPoint>]
let main args = 
  let result = ref OKAY
  try
    // initialize context
    use ctx = new Context 1
    // connect to server
    use socket = req ctx
    connect socket args.[0]
    // gather user input
    let mutable clock = 0n
    let mutable msg = prompt "enter a message and press <return>:"
    while not <| System.String.IsNullOrWhiteSpace msg do
      // send message to server
      clock <- zmq_stopwatch_start()
      msg |> encode |>> socket  
      // display server's reply
      let reply = socket |> recv |> decode
      let time  = float (zmq_stopwatch_stop clock)
      printfn "(%3f ms) %s " (time / 1000.0) reply
      // lather. rinse. repeat.
      msg <- prompt "message?"
  with
  | x ->  result := FAIL
          printfn "FAIL: %s" x.Message
            
  printf "press <return> to exit... "
  scanln () |> ignore
  exit !result
