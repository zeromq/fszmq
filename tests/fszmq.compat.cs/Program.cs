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
    const String REQUEST_ADDRESS = @"inproc://request.test";

    const Int32 ONE_SECOND = 1000;

    static readonly Byte[][] message = new [] { new Byte[0], new Byte[0], new Byte[0] };
    
    static Boolean doLoop = true;

    static void Monitor (Object obj)
    {
      var context = obj as Context;
      if (context == null) throw new InvalidOperationException("Invalid Context");
      using (var monitor = context.Pair())
      { 
        monitor.Connect(MONITOR_ADDRESS);
        while (doLoop)
        {
          try
          {
            var evt = monitor.RecvEvent();
            Debug.WriteLine("{0} ({1})", evt.Details, evt.Address);
            if (evt.Details.IsMonitorStopped) break;
          }
          catch (ZMQError x)
          { 
            Debug.WriteLine(string.Format("ERROR: {0}", x.Message));
          }
        }
      }
    }

    static void Client (Object obj)
    {
      var context = obj as Context;
      if (context == null) throw new InvalidOperationException("Invalid Context");
      using (var socket = context.Req())
      { 
        socket.Connect(REQUEST_ADDRESS);

        var count = 0;
        socket.SendAll(message);
        Console.WriteLine("[{0}] Message sent, awaiting reply", count);   

        var msg = new Byte[0][];
        while (doLoop && socket.TryGetInput(2500,out msg))
        {
          Console.WriteLine("[{0}] Received reply", count);
          count += 1;
          socket.SendAll(message);
          Console.WriteLine("[{0}] Message sent, awaiting reply", count);   
        }
      }
    }

    static void Main (String[] args)
    {
      using (var context = new Context())
      using (var socket  = context.Rep())
      {  
        socket.Monitor(MONITOR_ADDRESS,ZMQ.EVENT_ALL);
        (new Thread(Monitor)).Start(context);
        
        socket.Bind(REQUEST_ADDRESS);
        (new Thread(Client)).Start(context);

        Console.CancelKeyPress += (_,e) => { doLoop   = false; 
                                             e.Cancel = false; };
        while (doLoop)
        {
          Thread.Sleep(ONE_SECOND / 2);
          var msg = socket.RecvAll();
          Console.WriteLine("Got {0} bytes", msg.Sum(f => f.Length));
          socket.SendAll(new []{ new []{ (byte)0 } });
        }
      }
    }
  }
}
