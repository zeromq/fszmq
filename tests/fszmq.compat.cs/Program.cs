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
            Debug.WriteLine("MONITOR: {0} ({1})", evt.Details, evt.Address);
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
      using (var context  = new Context())
      using (var socket   = context.Rep())
      using (var replyMsg = new Message(new []{ (byte)0 }))
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
          replyMsg.Send(socket);
        }
      }
    }
  }
}
