Contributing to fszmq
===

Contributing to fszmq is fairly stright-forward. We use the [Collective Code Construction
Contract](http://rfc.zeromq.org/spec:22) (hereafter [C4.1](http://rfc.zeromq.org/spec:22)).

But summarizing briefly:

1.  Fork this repository

2.  Identify an issue from the [issue tracker](https://github.com/zeromq/fszmq/issues)

4.  Submit a _Pull Request_ addressing the issue

---

### Developer Pre-requisites

fszmq targets version 4.0, and above, of the .NET runtime, or the equivalent version of the Mono run-time
(although it's been known to run on earlier versions). Additionally, the native ZeroMQ library (libzmq)
will need to be available (the best way to provide this varies by platform).

The repository currently maps the source files to two (2) different sets of project files. For development on Windows,
please use the `fszmq-win.sln` file with Visual Studio 2012 or greater. Meanwhile on Mac OS X, use the `fszmq-osx` file 
with Xamarin Studio 5 or greater. _PLEASE NOTE: the actual source is cross-platform and can be compiled on *any* OS.
The multiple platforms contained in this repo are merely to simplify (and encourage) development._

Within the code base, the following libraries are used (though a developer SHOULD NOT worry about having them 
pre-installed, as bootstrapping is handled automatically):

	+ [FAKE]() ... for build-automation
	+ [Paket]() ... for dependency management
	+ [FSharp.Formatting]() ... for API documentation and help files (include the zguide samples)
	+ [NUnit]() ... for unit testing

---------------------------------------------------------------------------

**Be Advised**
The _native_ ZeroMQ library file (libzmq) will need to be made available for the 2.1.x and 3.2.x branches to run.
Starting with the the 4.0.x release of 0MQ (currently, the Master branch of fszmq), copies of libzmq are included 
with the project source. This version of libzmq does NOT support OpenMP or Sodium. It is generally meant to kick-start 
development and, especially on non-Windows OSes, will likely be replaced when your code is deployed to production.

---------------------------------------------------------------------------

For developing, open the `.sln` file in the IDE appropriate to your operating system, and have at it.

For building (non-debug scenarios), open a command prompt and type: `./build.sh`

### Repository Overview

//TODO: ???

##### Solution Overview

//TODO: ???

### Coding guidelines

* ???

#### Testing

_Please note: a full testing suite is under development, but is (woefully) incomplete at this time._

#### Platforms

At this point in time, fszmq has been tested on the following platform/architecture/runtime combinations:
* 32-bit Windows XP (running against .NET)
* 64-bit Windows Server 2008 R2 (running against .NET)
* 32-bit and 64-bit Windows 7 (running against .NET or Mono)
* 32-bit and 64-bit Windows 8 (running against .NET or Mono)
* 64-bit OS X 10.9.4 (running against Mono)

Other platform/architecture/runtime combinations should work (so long as .NET or Mono are supported and
there is a native libzmq port) and will be tested in due course..

_Please note: comprehensive Unix testing has NOT been conducted, due to resource constraints._

_Please note: comprehensive Linux testing has NOT been conducted, due to resource constraints._
