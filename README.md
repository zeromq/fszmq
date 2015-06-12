<!---
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
-->
fszmq
=======================

### An F# binding for the ZeroMQ distributed computing library.

fszmq is an MPLv2-licensed F# binding for the ZeroMQ distributed computing library.
It provides a complete binding to versions 2.1.x, 3.2.x, and 4.0.x of ZeroMQ
(Note: each binding is a separate branch in git, as there are some non-compatible differences).
This library is primarily designed to be consumed from F#. However, where possible, the library has been designed
to appear "friendly" when consumed by other CLR languages (C#, et aliam).

---------------------------------------------------------------------------

### NuGet

The fszmq library can be [installed from NuGet](https://www.nuget.org/packages/fszmq):

<pre>PM> Install-Package fszmq</pre>

_Please note: the fszmq NuGet package **ONLY** provides the managed `fszmq.dll` file.
**You** need to provide the native `libzmq` file(s) appropriate to your platform._

Additionally, on **Windows** you can use [this package](https://www.nuget.org/packages/fszmq.plus):

<pre>PM> Install-Package fszmq.plus</pre>

_Please note: the fszmq.plus NuGet package will attempt to include a version of `libzmq.dll`
(either x86 or x64/AnyCPU, depending on configuration) with your project._

---------------------------------------------------------------------------

### Library design

fszmq mostly follows the ZeroMQ guidelines for language bindings, and uses an approach similar to many C libraries.
Specifically, the three core ZeroMQ "concepts" (`Context`, `Socket`, and `Message`) are each modelled as a type
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
open an [issue](http://github.com/zeromq/fszmq/issues) and it'll get sorted.

---------------------------------------------------------------------------

Issues, questions, and concerns may be directed the the [Issue Tracker](http://github.com/zeromq/fszmq/issues).

More information about ZeroMQ is available at http://zero.mq.

If you'd like to help develop and maintain fszmq, please read about [CONTRIBUTING](CONTRIBUTING.MD).

---------------------------------------------------------------------------

###### This project is released under the MPL (v2) [license](LICENSE.txt).
###### This project's documentation is released under the MIT [license](docs/files/LICENSE.txt).
