open System
Environment.CurrentDirectory <- 
  __SOURCE_DIRECTORY__ + @"..\..\..\lib\zeromq"

//#I @"..\..\lib\fseye"
//#load "fseye.fsx"
//eye.Show()

#load "Native.fs"
open fszmq
#load "Constants.fs"
open fszmq
#load "Core.fs"
open fszmq
#load "Marshal.fs"
open fszmq
#load "Socket.fs"
open fszmq
open fszmq.Socket
#load "Context.fs"
open fszmq
open fszmq.Context
#load "Polling.fs"
open fszmq
open fszmq.Poll

#time "on"

let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString

printfn "%A" ZMQ.version
