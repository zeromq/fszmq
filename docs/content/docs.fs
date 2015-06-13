module docs

open Microsoft.FSharp.Core.Printf
open System
open System.Threading
open System.Text

// general helpers
type frame = byte array
let inline dispose (d:#IDisposable) = match d with null -> () | _ -> d.Dispose ()

// text helpers
let inline encode value = value |> string |> System.Text.Encoding.ASCII.GetBytes
let inline decode value = System.Text.Encoding.ASCII.GetString value
let hexstr (frame:frame) =
  frame
  |> Array.fold (fun b f -> bprintf b "%02x" f; b)
                (StringBuilder (2 * Array.length frame))
  |> string

// threading helpers
let inline sleepms ms = Thread.Sleep(int ms)
let inline spawn  fn   = Thread(ThreadStart fn).Start()
let inline spawnp fn o = Thread(ParameterizedThreadStart fn).Start(o)

// global environment helpers
module PATH =
  let private divider =
    match Environment.OSVersion.Platform with
    | PlatformID.Unix
    | PlatformID.MacOSX -> ":"
    | _                 -> ";"
  let private zmqFolder =
    match Environment.OSVersion.Platform with
    | PlatformID.Unix
    | PlatformID.MacOSX ->  sprintf "%s/../../bin" __SOURCE_DIRECTORY__
    | _                 ->  //NOTE: for now, WIN docs are 32-bit only
                            sprintf "%s/../../bin/x86" __SOURCE_DIRECTORY__
  let private oldPATH = ref ""
  // temporarily add location of native libs to global environment
  let hijack () =
    oldPATH := Environment.GetEnvironmentVariable "PATH"
    let newPATH = sprintf "%s%s%s" !oldPATH divider zmqFolder
    Environment.SetEnvironmentVariable ("PATH",newPATH)
  // undo changes to global environment
  let release () =
    if not <| String.IsNullOrWhiteSpace !oldPATH then
      Environment.SetEnvironmentVariable ("PATH",!oldPATH)
      oldPATH := ""
