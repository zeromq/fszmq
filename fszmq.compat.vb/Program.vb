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
Imports Microsoft.FSharp.Core.OptionModule
Imports System.Threading

Module Program

  Const MONITOR_ADDR = "inproc://monitor.rep"
  Const REQUEST_ADDR = "tcp://127.0.0.1:1979"

  Sub Monitor(obj As Object)
    Dim ctx = DirectCast(obj,Context)
    Using mon As Socket = ctx.Pair()
      mon.Connect(MONITOR_ADDR)
      Dim doLoop = True
      While doLoop
        Dim evt = mon.NextEvent()
        If IsSome(evt) Then
          Dim data = GetValue(evt).Data
          Select True
            Case Data.Tag = SocketEventData.Tags.Listening
              Dim listen = DirectCast(data,SocketEventData.Listening)
              Console.WriteLine("PID {1} is listening on {0}",
                                listen.Item1,
                                listen.Item2)
            Case Else
              Console.WriteLine(data)
          End Select
        End If
      End While
    End Using
  End Sub

  Sub Main()
    Using ctx As New Context(), 
          rep As Socket = ctx.Response()
      rep.CreateMonitor(MONITOR_ADDR,ZMQ.EVENT_ALL)
      Dim thr = New Thread(AddressOf Monitor)
      thr.Start(ctx)
      rep.Bind(REQUEST_ADDR)
    ' give events a chance to fire
      Thread.Sleep(TimeSpan.FromSeconds(1))
    End Using
  End Sub

End Module
