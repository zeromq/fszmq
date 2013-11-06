'--------------------------------------------------------------------------
'Copyright (c) Paulmichael Blasucci.                                        
'
'This source code is subject to terms and conditions of the Apache License, 
'Version 2.0. A copy of the license can be found in the License.html file   
'at the root of this distribution.                                          
'
'By using this source code in any fashion, you are agreeing to be bound     
'by the terms of the Apache License, Version 2.0.                           
'
'You must not remove this notice, or any other, from this software.         
'--------------------------------------------------------------------------
Imports fszmq.ContextModule
Imports fszmq.SocketModule
Imports System.Threading

Module Program

  Const REQUEST_ADDR = "tcp://127.0.0.1:1979"

  Sub Main()
    Using context As New Context(), socket = context.Request()
      
      socket.Connect(REQUEST_ADDR)
      
      Dim frame   As Byte()   = {0}
      Dim message As Byte()() = {frame,frame,frame}
      
      For counter = 1 to 10
        socket.SendAll(message)   
        Console.WriteLine("Message sent, awaiting reply")
        socket.RecvAll() ' ignore
        Console.WriteLine("Received reply")
        Thread.Sleep(500)
      Next
    
    End Using
  
  End Sub

End Module
