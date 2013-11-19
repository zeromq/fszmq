/* ------------------------------------------------------------------------
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
------------------------------------------------------------------------ */
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
