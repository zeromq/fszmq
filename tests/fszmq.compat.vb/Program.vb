' -------------------------------------------------------------------------
' This file is part of fszmq.
' 
' fszmq is free software: you can redistribute it and/or modify
' it under the terms of the GNU Lesser General Public License as published 
' by the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' fszmq is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
' GNU Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public License
' along with fszmq. If not, see <http://www.gnu.org/licenses/>.
' 
' Copyright (c) 2011-2013 Paulmichael Blasucci
' -------------------------------------------------------------------------
Imports fszmq.ContextModule
Imports fszmq.SocketModule
Imports fszmq.PollingExtensions
Imports fszmq.PollingModule

Imports System.Threading

Module Program

  Const REQUEST_ADDR = "tcp://127.0.0.1:1979"

  Dim Frame   As Byte()   = {0}
  Dim Message As Byte()() = {Frame,Frame,Frame}
  
  Sub MainPoll()

    Using context As New Context(), socket = context.Request()
      
      socket.Connect(REQUEST_ADDR)
      
      socket.SendAll(Message)
      Console.WriteLine("Message sent, awaiting reply")      

      While DoPoll(500,New Poll() { socket.AsPollIn(Sub (__) socket.RecvAll()) } )
        Console.WriteLine("Received reply")
        socket.SendAll(Message)
        Console.WriteLine("Message sent, awaiting reply")      
      End While

    End Using

  End Sub

  Sub Main()

    Using context As New Context(), socket = context.Request()
      
      socket.Connect(REQUEST_ADDR)
      
      For counter = 1 to 10
        socket.SendAll(Message)   
        Console.WriteLine("Message sent, awaiting reply")
        socket.RecvAll() ' ignore
        Console.WriteLine("Received reply")
        Thread.Sleep(500)
      Next
    
    End Using
  
  End Sub

End Module
