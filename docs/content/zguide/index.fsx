(**
zguide
===================

[The zguide][zguide] is a well-written, exhaustive explanation of the library. 
It should definitely be read. And then read again. Also, while the zguide features examples in many languages, 
the [F#-specific zguide code samples][zguidefs] have been collected here as a convenience.

--------------------

 * [Hello World server](hwserver.html) ... Expects "Hello" from client, replies with "World"
 * [Hello World client](hwclient.html) ... Sends "Hello" to server, expects "World" back
 
 * [0MQ Version](version.html) ... Displays the version of 0MQ currently being used
 
 * [Weather Update server](wuserver.html) ... Publishes random weather updates
 * [Weather Update client](wuclient.html) ... Collects weather updates and finds avg temp in zipcode

 * [Task worker](taskwork.html) ... Collects workloads from ventilator and sends results to sink
 * [Task ventillator](taskvent.html) ... Sends batch of tasks to workers
 * [Task sink](tasksink.html) ... Collects results from workers
 
 * more to come

--------------------

The documentation accompanying fszmq, and any sample code contained therein, is available under
the MIT LICENSE which allows modification and reuse for both commercial non-commercial purposes.
For more information see the [documentation license file][docslicense] in the GitHub repository. 

  [docslicense]: https://github.com/pblasucci/fszmq/blob/master/docs/files/LICENSE.txt
  [zguide]: http://zguide.zeromq.org/page:all
  [zguidefs]: https://github.com/imatix/zguide/tree/master/examples/F%23
*)