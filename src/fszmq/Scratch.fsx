(* ------------------------------------------------------------------------
This file is part of fszmq.

fszmq is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published 
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

fszmq is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with fszmq. If not, see <http://www.gnu.org/licenses/>.

Copyright (c) 2011-2013 Paulmichael Blasucci
------------------------------------------------------------------------ *)

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
#load "Marshal.fs"
open fszmq
#load "Core.fs"
open fszmq
#load "Message.fs"
open fszmq
open fszmq.Message
#load "Socket.fs"
open fszmq
open fszmq.Socket
#load "Context.fs"
open fszmq
open fszmq.Context
#load "Polling.fs"
open fszmq
open fszmq.Polling
#load "Utilities.fs"
open fszmq
open fszmq.Proxying

(*-----------------------------------------------------------------------*)

#time "on"

printfn "%A" ZMQ.version

(*-----------------------------------------------------------------------*)

open System.Text

let encode data = Encoding.ASCII.GetBytes(string data)
let decode data = Encoding.ASCII.GetString(data)
  
let hexstr frame =
  frame
  |> Array.fold (fun (b:StringBuilder) (f:byte) -> b.AppendFormat("{0:2X}",f))
                (StringBuilder(2 * Array.length frame))
  |> string

(*-----------------------------------------------------------------------*)

