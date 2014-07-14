(*** do-not-eval-file ***)
(*** hide ***)
#I "../../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
Weather Update Client
====================

Connects SUB socket to tcp://localhost:5556

Collects weather updates and finds avg temp in zipcode
*)

#r "fszmq.dll"
open fszmq

// helpers to convert between strings and frames
let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString

let main args = 
  use context = new Context ()

  // socket to talk to server
  printfn "Collecting updates from weather server..."
  use subscriber = Context.sub context
  Socket.connect subscriber "tcp://localhost:5556"

  // subscribe to zipcode, default is NYC, 10001
  let filter =  match Array.length args with
                | n when n >= 1 -> Array.get args 0
                | _             -> "10001"
  Socket.subscribe subscriber [ encode filter ]

  printfn "%A" filter

  // process 100 updates
  let total_temp = ref 0
  let update_nbr = ref 0
  for _ in 0 .. 99 do
    let update = decode <| Socket.recv subscriber
    // update = "zipcode temperature relhumidity"
    let temperature = int <| Array.get (update.Split ()) 1                  
    total_temp := !total_temp + temperature
    incr update_nbr

  printfn "Average temperature for zipcode '%s' was %dF"
          filter
          (!total_temp / !update_nbr)              
  
  0 // return code

(*** hide ***)    
main fsi.CommandLineArgs.[1 ..]
// 0th commandline arg is __SOURCE_FILE__
