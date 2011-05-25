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

open System.Runtime.InteropServices

[<AutoOpen>]
module internal NativeMemory =

  type NativeMemory(size:int) =
  
    let mutable disposed  = false
    let mutable _memory   = Marshal.AllocHGlobal(size)

    do if _memory = 0n then failwith "unable to initialize native memory"
     
    let release () =
      if not disposed then
        disposed <- true
        Marshal.FreeHGlobal(_memory)
        _memory <- 0n

    member __.Handle  = _memory
    member __.Size    = size |> unativeint
  
    override __.Finalize() = release ()

    interface System.IDisposable with

      member self.Dispose() =
        self.Finalize()
        System.GC.SuppressFinalize(self)
  

  let inline bool (o:^t) = System.Convert.ToBoolean(o)
  let inline long (o:^t) = System.Convert.ToInt64(o)

  let inline nativeMem (size:^t) = new NativeMemory(int size)

  let writeNative32 data (mem:NativeMemory) = 
    Marshal.WriteInt32(mem.Handle,data)
    mem

  let writeNative64 data (mem:NativeMemory) = 
    Marshal.WriteInt64(mem.Handle,data)
    mem

  let fillNative (data:byte[]) (mem:NativeMemory) = 
    Marshal.Copy(data,0,mem.Handle,data.Length)
    mem

  let inline extractNative (size:^t) (mem:NativeMemory) = 
    let size' = int size
    let value = Array.zeroCreate<byte> size'
    Marshal.Copy(mem.Handle,value,0,size')
    value

  let (|Int|Long|ULong|Bool|Binary|Other|) (t:System.Type) =
    if    t = typeof<byte[]>  then Binary
    elif  t = typeof<int32>   then Int
    elif  t = typeof<bool>    then Bool
    elif  t = typeof<int64>   then Long
    elif  t = typeof<uint64>  then ULong
                              else Other
