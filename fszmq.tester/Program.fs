namespace fszmq.tester

open fszmq
open fszmq.Context
open fszmq.Socket
open fszmq.Polling

open System.Text.RegularExpressions

module Program =
  
  let [<Literal>] OKAY = 0

  [<EntryPoint>]
  let main _ = 
    //TODO: ???
    OKAY
