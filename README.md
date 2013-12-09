<!---
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
-->
fszmq
=======================

---------------------------------------------------------------------------

### An F# binding for the ZeroMQ distributed computing library.

fszmq is an LGPLv3-licensed F# binding for the ZeroMQ distributed computing library. 
It provides a complete binding to versions 2.1.x, 3.2.x, and 4.0.x of ZeroMQ 
(Note: each binding is a separate branch in git, as there are some non-compatible differences). 
This library is primarily designed to be consumed from F#. However, where possible, the library has been designed 
to appear "friendly" when consumed by other CLR languages (C#, et aliam).

---------------------------------------------------------------------------

#### Library design

fszmq mostly follows the ZeroMQ guidelines for language bindings, and uses an approach similar to many C libraries.
Specifically, the three core ZeroMQ ""concepts" (`Context`, `Socket`, and `Message`) are each modelled as a type 
with a definite lifetime (i.e. must be instantiated and implements `System.IDisposable`), which wraps a native resource.
However, all non-lifecycle operations are defined as functions in various modules (e.g. `fszmq.Socket`, `fszmq.Polling`).
Nearly all of these module functions expect an instance of (at least) one of the core types as input. 
Further, many of the module functions are extensions when used from CLR languages other than F# 
(i.e. they present as member functions on instances of `Context`, `Socket`, or `Message`). 
Finally, many relevant constant values are defined in the `fszmq.ZMQ` module.

#### A note about versions

The master branch of fszmq is tracked against the latest stable branch of ZeroMQ.
Separate (inactive) branches are created for major changes, and usually align with stable branches of ZeroMQ.
As a rule, the first segment of fszmq's versioning scheme _always_ matches the first segment of libzmq's versioning scheme
(e.g. fszmq version 3.x.y is tracked against libzmq version 3.x.y). Further, in practice, the second segement of fszmq's
versioning scheme _usually_ matches the equivalent in libzmq. While this can not be guaranteed, it has not been invalided yet.
However, the third segment of fszmq's version is _in no way correlated_ to any part of libzmq. The third segment tracks 
semanitcally minor changes to the actual fszmq codebase. The practical take-away from all this is: 
so long as the major versions agree, you're not likely to have no problems between fszmq and libzmq. If you do, 
open an [issue](http://github.com/pblasucci/fszmq/issues) and it'll get sorted.

#### Testing

_Please note: a full testing suite is under development, but is (woefully) incomplete at this time._

_Please note: comprehensive documentation is under development, but is incomplete at this time._

---------------------------------------------------------------------------

### Platforms

At this point in time, fszmq has been tested against the CLR on the following platform/architecture/runtime combinations:
* 32-bit Windows XP (running against .NET)
* 64-bit Windows Server 2008 R2 (running against .NET)
* 32-bit and 64-bit Windows 7 (running against .NET or Mono)
* 32-bit and 64-bit Windows 8 (running against .NET or Mono)

Other platform/architecture/runtime combinations should work (so long as the .NET or Mono are supported and 
there is a native libzmq port) and will be tested in due course..

_Please note: comprehensive Mac OS X testing has NOT been conducted, due to resource constraints._
_Please note: comprehensive Unix testing has NOT been conducted, due to resource constraints._
_Please note: comprehensive Linux testing has NOT been conducted, due to resource constraints._

### NuGet

The fszmq library can be [installed from NuGet](https://www.nuget.org/packages/fszmq):

<pre>PM> Install-Package fszmq</pre>

_Please note: the fszmq NuGet package will attempt to include a version of libzmq (either x86 or x64/AnyCPU, 
depending on configuration) with your project._

**Be Advised**
Several popular Unix-based operating systems (e.g. FreeBSD, Mac OSX) may exhibit strange behavior when sending and receiving 
(via the `Message` and `Socket` modules). This is due to alternate OS-level values for EAGAIN (and EWOULDBLOCK). To remedy this,
DO NOT USE THE BINARY DISTRIBUTION. Instead, compile fszmq from source with the conditional-compilation symbol `BSD_EAGAIN`.
In an IDE (e.g. Visual Studio, Xamarin Studio), this can be set in the project properties for fszmq. 
If you are using fsc.exe to compile from the command-line, include `--define:BSD_EAGAIN` as part of your command.

### Pre-requisites

fszmq targets version 4.0, and above, of the .NET runtime, or the equivalent version of the Mono run-time (although it's been known to run on earlier versions). 

fszmq (for the 2.1.x branch) should be compiled with Visual Studio 2010 SP1, 
the equivalent F# Free Tools Release, or a compatible version of MonoDevelop or Xamarin Studio.

fszmq (for the 3.2.x branch) should be compiled with Visual Studio 2012, 
the equivalent F# Free Tools Release, or a compatible version of MonoDevelop or Xamarin Studio.

fszmq (for the Master branch) should be compiled with Visual Studio 2013, 
the equivalent F# Free Tools Release, or a compatible version of MonoDevelop or Xamarin Studio.

**Be Advised** 
The ZeroMQ library file (libzmq) will need to be made available for the 2.1.x and 3.2.x branches to run. 
Starting with the current Master (targeting the 4.0.x release of 0MQ), copies of libzmq are included with the project source.

---------------------------------------------------------------------------

Issues, questions, and concerns may be directed the the [Issue Tracker](http://github.com/pblasucci/fszmq/issues).

More information about ZeroMQ is available at http://zero.mq.

---------------------------------------------------------------------------

###### Copyright &#169; 2011-2013 Paulmichael Blasucci. 
###### This project is released under the LGPL (v3) [license](COPYING.lesser).
###### This project's documentation is released under the MIT [license](docs/files/LICENSE.txt).
