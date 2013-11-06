/*-------------------------------------------------------------------------
Copyright (c) Paulmichael Blasucci.                                        
                                                                           
This source code is subject to terms and conditions of the Apache License, 
Version 2.0. A copy of the license can be found in the License.html file   
at the root of this distribution.                                          
                                                                           
By using this source code in any fashion, you are agreeing to be bound     
by the terms of the Apache License, Version 2.0.                           
                                                                           
You must not remove this notice, or any other, from this software.         
-------------------------------------------------------------------------*/
using fszmq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace fszmq.compat.cs
{
  class Program
  {
    const String MONITOR_ADDRESS = @"inproc://monitor.test";
    const String REQUEST_ADDRESS = @"tcp://127.0.0.1:1979";
    
    const Int32 ONE_SECOND = 1000;

    static void Monitor (Object obj)
    {
      var context = obj as Context;
      if (context == null) throw new InvalidOperationException("Invalid Context");
      using (var monitor = context.Pair())
      { 
        monitor.Connect(MONITOR_ADDRESS);
        while (true)
        {
          try
          {
            var evt = ZMQEvent.Build(monitor.RecvAll());
            Debug.WriteLine("{0} ({1})", evt.Details, evt.Address);
            if (evt.Details.IsMonitorStopped) break;
          }
          catch (ZMQ.NotAnEvent x)
          { 
            Debug.WriteLine("ERROR: {0}", x.Message);
          }
        }
      }
    }

    static void Main (String[] args)
    {
      using (var context = new Context())
      using (var socket  = context.Response())
      {  
        socket.CreateMonitor(MONITOR_ADDRESS,ZMQ.EVENT_ALL);
        (new Thread(Monitor)).Start(context);
        socket.Bind(REQUEST_ADDRESS);
        var doLoop = true;
        Console.CancelKeyPress += (_,e) => { doLoop   = false;
                                             e.Cancel = false; };
        while (doLoop)
        {
          Thread.Sleep(ONE_SECOND / 2);
          var message = socket.RecvAll();
          Console.WriteLine("Got {0} bytes", message.Select(f => f.Length).Sum());
          socket.SendAll(new []{ new []{ (byte)0 } });
        }
      }
    }
  }
}
