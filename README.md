fszmq
=======================

### An F# binding for the ZeroMQ distributed computing library.

fszmq is an MPLv2-licensed F# binding for the ZeroMQ distributed computing library.
It provides a complete binding to versions 2.1.x, 3.2.x, 4.0.x, 4.1.x of ZeroMQ
(Note: prior to 4.0.0, each binding is a separate branch in git, as there are some non-compatible differences).
This library is primarily designed to be consumed from F#. However, where possible,
the library has been designed to appear "friendly" when consumed by other CLR languages (C#, et aliam).

---------------------------------------------------------------------------

### NuGet

The fszmq library can be [installed from NuGet](https://www.nuget.org/packages/fszmq):

<pre>PM> Install-Package fszmq</pre>

_Please note: the fszmq NuGet package **ONLY** provides the managed `fszmq.dll` file.
**You** need to provide the native `libzmq` file(s) appropriate to your platform._

---------------------------------------------------------------------------

### Acquiring `libzmq`

On Windows, the recommended solution is to [download a release from the official GitHub repo](https://github.com/zeromq/libzmq/releases) and then compile it in a recent version of Visual Studio (several versions are supported... check the release docs for more information).

On macOS, the recommended solution is to install `zmq` from [Homebrew](https://brew.sh/) -- the missing package manager.

On Linux, the recommended solution is install `libzmq` from your distro's package manager. Alternatively, you may [build from source](https://github.com/zeromq/libzmq).

_Please note: in a pinch, the `lib` folder in the root of this repo contains Windows, macOS, and Linux binaries of `libzmq`. But they're not compiled with all available features for all platforms and may not behave as expected._

### Platforms

At this point in time, fszmq has been tested on the following platform/architecture/runtime combinations:
* 32-bit Windows XP (running against .NET Framework)
* 64-bit Windows Server 2008 R2 (running against .NET Framework)
* 32-bit and 64-bit Windows 7 (running against .NET Framework or Mono)
* 32-bit and 64-bit Windows 8 (running against .NET Framework or Mono)
* 32-bit and 64-bit Windows 10 (running against .NET Framework, .NET Core, and Mono)
* 32-bit and 64-bit OS X (running against Mono)
* 64-bit Linux (Ubuntu 18.04 running against .NET Core)

Other platform/architecture/runtime combinations should work (so long as .NET Framework, .NET Core, or Mono are supported and
there is a native libzmq port).

---

> **IMPORTANT NOTE:** starting with version 12.3, `fszmq` will only ship as a .NET Standard library (currently, .NET Standard 2). 
> This _may_ have consequences (or limitations) for existing applications wishing to upgrade.

---

#### A note about versions

In the past, several attempts were made (poorly) to keep the version of `fszmq` synchronized to the version of `libzmq`.
Starting with verion `12.0.0` (which is the _twelth_ release... version 4.0.9 was the eleventh release), this is no longer done. 
Instead, `fszmq` is versioned _independently_. The versions of `libzmq` supported by each version of `fszmq` are as follows
(where 'x' means any number >= 0):

`fszmq` version | `libzmq` versions supported
---------------:|----------------------------:
12.3.0          | 4.1.x (and a sub-set of 4.2.x)
12.2.x          | 4.1.x
12.1.1          | 4.1.x
12.1.0          | 4.1.x
12.0.1          | 4.0.x
4.0.9           | 4.0.5
4.0.8           | 4.0.5
4.0.6           | 4.0.5
4.0.5           | 4.0.5
4.0.4           | 4.0.4
3.2.7           | 3.2.5
3.2.5           | 3.2.5
2.2.1           | 2.2.1
2.2.0           | 2.2.0
2.1.11          | 2.1.x
2.1.10          | 2.1.x
2.1.6           | 2.1.x

---------------------------------------------------------------------------

More information about ZeroMQ is available at http://zero.mq.

Issues, questions, and concerns may be directed the the [Issue Tracker](http://github.com/zeromq/fszmq/issues).

If you'd like to help develop and maintain fszmq, please read about [CONTRIBUTING](CONTRIBUTING.md).

---------------------------------------------------------------------------

###### This project is released under the MPL (v2) [license](LICENSE.txt).
###### This project's documentation is released under the MIT [license](docs/files/LICENSE.txt).
