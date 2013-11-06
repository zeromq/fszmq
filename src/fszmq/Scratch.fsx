(*-------------------------------------------------------------------------
Copyright (c) Paulmichael Blasucci.

This source code is subject to terms and conditions of the Apache License,
Version 2.0. A copy of the license can be found in the License.html file
at the root of this distribution.

By using this source code in any fashion, you are agreeing to be bound
by the terms of the Apache License, Version 2.0.

You must not remove this notice, or any other, from this software.
-------------------------------------------------------------------------*)

// set up environment
open System

Environment.CurrentDirectory <- 
  sprintf @"%s..\..\..\lib\zeromq\%s"
          __SOURCE_DIRECTORY__
          (if Environment.Is64BitProcess then "x64" else "x86")

printfn "CurrentDirectory = %s" Environment.CurrentDirectory

(*-----------------------------------------------------------------------*)

// load working code
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
open fszmq.Polling
#load "Proxy.fs"
open fszmq
open fszmq.Proxying

(*-----------------------------------------------------------------------*)

#time "on"

let encode = string >> System.Text.Encoding.ASCII.GetBytes
let decode = System.Text.Encoding.ASCII.GetString

printfn "%A" ZMQ.version

(*-----------------------------------------------------------------------*)
