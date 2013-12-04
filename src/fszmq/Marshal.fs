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
namespace fszmq

open System
open System.Runtime.InteropServices
open System.Text

[<AutoOpen>]
module internal Marshal =

  //NOTE: this isn't really a native/managed marshalling function.
  //      this was just a convenient place to define it.
  let inline bool (v:^T) = Convert.ToBoolean(v)

(* general-purpose helpers *)
  let inline useBuffer fn (size:int) =
    let buffer = Marshal.AllocHGlobal(size)
    try
      fn (unativeint size,buffer)
    finally
      Marshal.FreeHGlobal(buffer)
   
(* reading native values *)  
  let inline readInt32 pointer = Marshal.ReadInt32(pointer)
  let inline readInt64 pointer = Marshal.ReadInt64(pointer)

  let inline readBool   pointer = (readInt32 >> Convert.ToBoolean) pointer
  let inline readUInt64 pointer = (readInt64 >> Convert.ToUInt64 ) pointer

  let inline readBytes (length,pointer) =
    let length = int length
    let value  = Array.zeroCreate<byte> length
    Marshal.Copy(pointer,value,0,length)
    value

  let inline readString (length,pointer) = Encoding.UTF8.GetString(readBytes (length,pointer))

(* writing native values *)
  let inline writeInt32 value pointer = Marshal.WriteInt32(pointer,value)
  let inline writeInt64 value pointer = Marshal.WriteInt64(pointer,value)

  let inline writeBool value pointer = writeInt32 (if value then 1 else 0) pointer
  let inline writeUInt64 (value:UInt64) pointer = writeInt64 (int64 value) pointer
  
  let inline writeBytes (value:byte[]) pointer = Marshal.Copy(value,0,pointer,value.Length)

  let inline writeString value pointer =
    let bytes = System.Text.Encoding.UTF8.GetBytes(string value) 
    Marshal.Copy(bytes,0,pointer,bytes.Length)
