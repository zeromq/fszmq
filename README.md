fszmq
=======================

### An F# binding for the ZeroMQ distributed computing library.

fszmq is an MPLv2-licensed F# binding for the ZeroMQ distributed computing library.
It provides a complete binding to versions 2.1.x, 3.2.x, and 4.0.x of ZeroMQ
(Note: each binding is a separate branch in git, as there are some non-compatible differences).
This library is primarily designed to be consumed from F#. However, where possible,
the library has been designed to appear "friendly" when consumed by other CLR languages (C#, et aliam).

---------------------------------------------------------------------------

### NuGet

The fszmq library can be [installed from NuGet](https://www.nuget.org/packages/fszmq):

<pre>PM> Install-Package fszmq</pre>

_Please note: the fszmq NuGet package **ONLY** provides the managed `fszmq.dll` file.
**You** need to provide the native `libzmq` file(s) appropriate to your platform._

---------------------------------------------------------------------------

### Platforms

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

#### A note about versions

The master branch of fszmq is tracked against the latest stable branch of ZeroMQ.
Separate (inactive) branches are created for major changes, and usually align with stable branches of ZeroMQ.
As a rule, the first segment of fszmq's versioning scheme _always_ matches the first segment of libzmq's versioning scheme
(e.g. fszmq version 3.x.y is tracked against libzmq version 3.x.y). Further, in practice, the second segement of fszmq's
versioning scheme _usually_ matches the equivalent in libzmq. While this can not be guaranteed, it has not been invalided yet.
However, the third segment of fszmq's version is _in no way correlated_ to any part of libzmq. The third segment tracks
semanitcally minor changes to the actual fszmq codebase. The practical take-away from all this is:
so long as the major versions agree, you're not likely to have no problems between fszmq and libzmq. If you do,
open an [issue](http://github.com/zeromq/fszmq/issues) and it'll get sorted.

---------------------------------------------------------------------------

Issues, questions, and concerns may be directed the the [Issue Tracker](http://github.com/zeromq/fszmq/issues).

More information about ZeroMQ is available at http://zero.mq.

If you'd like to help develop and maintain fszmq, please read about [CONTRIBUTING](CONTRIBUTING.md).

---------------------------------------------------------------------------

###### This project is released under the MPL (v2) [license](LICENSE.txt).
###### This project's documentation is released under the MIT [license](docs/files/LICENSE.txt).
