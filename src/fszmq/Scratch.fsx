#I @"..\..\lib\zeromq-2.1"
// NOTE:  changing the current directory is the easiest way to ensure
//        the (native) libzmq.dll is available for use in the REPL 
System.Environment.CurrentDirectory <- 
  __SOURCE_DIRECTORY__ + @"\..\..\lib\zeromq-2.1"

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

let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString
