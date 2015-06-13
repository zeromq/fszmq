(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
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
  let inline useBuffer (size:int) fn =
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
