module docs

open System
open System.Threading

// I/O helpers
let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString 
// threading helpers
let sleepms ms  = Thread.Sleep(int ms)
let spawn  fn   = Thread(ThreadStart fn).Start()
let spawnp fn o = Thread(ParameterizedThreadStart fn).Start(o)

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
