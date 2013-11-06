(*-------------------------------------------------------------------------
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*)
namespace fszmq

open System
open System.Runtime.InteropServices
open System.Text

[<AutoOpen>]
module internal Marshal =

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
