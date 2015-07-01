(**
zguide
===================

[The zguide][zguide] is a well-written, exhaustive explanation of the library.
It should definitely be read. And then read again. Also, while the zguide features examples in many languages,
the [F#-specific zguide code samples][zguidefs] have been collected here as a convenience.

_Please note: the zguide covers version 3.2.x of ZeroMQ, while the examples below have been modified to run against version 4.0.x of ZeroMQ._

--------------------

**Chapter 1 examples**

 * [Hello World server](hwserver.html) ... Expects "Hello" from client, replies with "World"
 * [Hello World client](hwclient.html) ... Sends "Hello" to server, expects "World" back

 * [0MQ Version](version.html) ... Displays the version of 0MQ currently being used

 * [Weather Update server](wuserver.html) ... Publishes random weather updates
 * [Weather Update client](wuclient.html) ... Collects weather updates and finds avg temp in zipcode

 * [Task worker](taskwork.html) ... Collects workloads from ventilator and sends results to sink
 * [Task ventillator](taskvent.html) ... Sends batch of tasks to workers
 * [Task sink](tasksink.html) ... Collects results from workers

--------------------

**Chapter 2 examples**

 * [Multi-socket Reader](msreader.html) ... Reads from multiple sockets using a simple recv loop
 * [Multi-socket Poller](mspoller.html) ... Reads from multiple sockets using ZMQ's polling functionality

 * [Request-Reply Client](rrclient.html) ... Sends "Hello" to a server (via a broker) and expects "World" back
 * [Request-Reply Worker](rrworker.html) ... Expects "Hello" from clients (via a broker) and replies with "World"
 * [Request-Reply Broker](rrbroker.html) ... A simple broker for connecting client requests to server replies

 * [Message Queue Broker](msgqueue.html) ... Like [Request-Reply Broker] but using device (i.e. a proxy)

 * [Weather Update Proxy](wuproxy.html) ... A proxy device which does network bridging

 * [Task Worker (2nd design)](taskwork2.html) ... Like [Task Worker](taskwork.html), but with a extra flow to receive and respond to a 'KILL' signal
 * [Task Sink (2nd design)](tasksink2.html) ... Like [Task Sink](tasksink.html), but with a extra flow to receive and respond to a 'KILL' signal

 * [Interrupt Handling](interrupt.html) ... Shows how to handle manual interrupt (i.e. CTRL+C) in a console application

 * [Multi-threaded Service](mtserver.html) ... Passes actual handling of requests to workers on other threads

 * [Signaling Between Threads](mtrelay.html) ... Demonstrates inter-thread coordination

 * [Synchronized Publisher](syncpub.html) ... Synchronizes data distribution across a fixed number of clients
 * [Synchronized Subscriber](syncsub.html) ... Participates in node-coordinated data distribution

 * [Pub-Sub Envelope Publisher](psenvpub.html) ... Sends multi-part messages where the first frame is the subscription topic
 * [Pub-Sub Envelope Subscriber](psenvsub.html) ... Only receives messages with a specific topic frame

--------------------

**Chapter 3 examples**

 * _Coming soon._

--------------------

**Chapter 4 examples**

 * _Coming soon._

--------------------

**Chapter 5 examples**

 * _Coming soon._

--------------------

The documentation accompanying fszmq, and any sample code contained therein, is available under
the MIT LICENSE which allows modification and reuse for both commercial non-commercial purposes.
For more information see the [documentation license file][docslicense] in the GitHub repository.

---------------------------------------------------------------------------

  [docslicense]: https://github.com/zeromq/fszmq/blob/master/docs/files/LICENSE.txt
  [zguide]: http://zguide.zeromq.org/page:all
  [zguidefs]: https://github.com/imatix/zguide/tree/master/examples/F%23
*)
