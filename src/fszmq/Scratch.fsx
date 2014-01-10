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

//module Frame =
//  
//  open System.Text
//
//  let encode data = Encoding.ASCII.GetBytes(string data)
//  let decode data = Encoding.ASCII.GetString(data)
//  
//  let hexstr frame =
//    frame
//    |> Array.fold (fun (b:StringBuilder) (f:byte) -> b.AppendFormat("{0:2X}",f))
//                  (StringBuilder(2 * Array.length frame))
//    |> string

(*-----------------------------------------------------------------------*)

//[<AutoOpen>] 
//module Msg =
//
//  type frame    = byte[]
//  type message  = list<frame>
//
//  open Frame
//
//  let push msg frame = frame :: msg
//  
//  let append msg frame = msg @ [ frame ]
//  
//  let pop msg = 
//    match msg with 
//    | []    -> None 
//    | h::t  -> Some (h,t)
//  
//  let pushstr msg str = push msg (encode str)
//  
//  let appendstr msg str = append msg (encode str)
//  
//  let popstr msg = 
//    pop msg |> Option.map (fun (h,t) -> (decode h),t)
//  
//  let wrap msg frame = frame :: [||] :: msg
//  
//  let unwrap msg =  
//    match msg with 
//    | []    ->  None 
//    | h::t  ->  match t with 
//                | []      -> Some (h,t)
//                | [||]::t -> Some (h,t)
//                | _::_    -> Some (h,t)

open System
open System.Text

type bitutils = BitConverter 

//type Msg(encoding :Encoding) =
//
//  let frames = ref List.empty<byte[]>
//
//  let (>>=) frame lambda = 
//    frames := frame :: !frames
//    frame |> lambda
//  
//  member __.Frames = !frames
//
//  member __.Bind(data :byte[] ,lambda) = data                      >>= lambda     
//  member __.Bind(data :bool   ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :char   ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :float32,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :float  ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :int16  ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :int32  ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :int64  ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :uint16 ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :uint32 ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :uint64 ,lambda) = data |> bitutils.GetBytes >>= lambda
//  member __.Bind(data :string ,lambda) = data |> encoding.GetBytes >>= lambda
//
//  member msg.Return(_) = List.rev msg.Frames
//  
//  static member EmptyFrame = [||]
//  
//let req_3 = Msg Encoding.ASCII { 
//  let! address  = 12345678
//  let! _        = Msg.EmptyFrame
//  let! protocol = "MDPC-1"
//  let! command  = 0x012345 
//  return () }
//
//req_3 |> Seq.iter (printfn "%A")
