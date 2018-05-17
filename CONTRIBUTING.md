Contributing to fszmq
===

Contributing to fszmq is fairly stright-forward. 
We use the [Collective Code Construction Contract](http://rfc.zeromq.org/spec:22) (hereafter [C4.1](http://rfc.zeromq.org/spec:22)).

But summarizing briefly:

1.  Fork this repository
2.  Identify an issue from the [issue tracker](https://github.com/zeromq/fszmq/issues)
3.	Hack around with some code
4.  Submit a _Pull Request_ addressing the issue

---

### Developer Pre-requisites

fszmq targets .NET Standard (currently 2.0). As such, the primary dependency is .NET Core, version 2.0 or greater. 
However, some of the associated utilities (profiling and documenation generation) target .NET Framework (in addition to .NET Core).
Thus, it is also necessary to install .NET Framwork 4.6.1 (or greater) _or_ the equivalent version of the Mono Framework.

Within the code base, the following libraries are used 
(though a developer SHOULD NOT worry about having them pre-installed, as bootstrapping is handled automatically):

+ [FAKE](http://fsharp.github.io/FAKE/) ... for build-automation
+ [Paket](http://fsprojects.github.io/Paket/) ... for dependency management
+ [FSharp.Formatting](http://tpetricek.github.io/FSharp.Formatting/) ... for API documentation and help files (include the zguide samples)
+ [FsCheck](http://) ... the primary tool for testing library code
+ [Unquote](http://) ... for tests drilling into particular nooks and crannies (or when property-based testing is a poor fit)
+ [NUnit](http://www.nunit.org/) ... for tying together tests and connecting them to various runners

---------------------------------------------------------------------------

**Be Advised**
The _native_ ZeroMQ library file (libzmq) will need to be made available for the
2.1.x and 3.2.x branches to run. Starting with the the 4.0.x release of 0MQ
(currently, the Master branch of fszmq), copies of libzmq are included with the
project source. This version of libzmq does NOT support OpenMP or Sodium. It is
generally meant to kick-start development and, especially on non-Windows OSes,
will likely be replaced when your code is deployed to production.

---------------------------------------------------------------------------

**For developing, open the `fszmq.sln` file in your favorite IDE, or open the root folder in your favorite editor, and have at it.**

**For building (non-debug scenarios), open a command prompt and type: `./build.sh`**

### Repository Overview

Although there are many, many files in the repository, most are infrastructural and can be safely ignored. 
The interesting ones are as follows:

Path                      | Purpose
--------------------------|-------------------------------------------------------------------------------
`RELEASE_NOTES.md`        | Use this to keep a record of new features or bug fixes
`src/fszmq/*.*`           | The actual source code files of `fszmq.dll` live in this project
`tests/fszmq.tests/*.*`   | The files in this project form the primary test suite for `fszmq.dll`
`docs/content/*.*`        | Any narrative-style documentation resides in this folder
`docs/content/zguide/*.*` | This folder contains examples specifically "borrowed" from the [zguide](http://zguide.zeromq.org/page:all)

### Coding guidelines

Code is read much more frequently than it is written. With that in mind, the following guidelines are intended 
to help maintain the readablity of fszmq's source. Please note, however, that these are not rules and 
the code may (rarely) deviate from them for a well-justifiable reason (especially in documentation files).

Remember: the ultimate goal is readablity (to ease maintenance).

##### General guidelines

* ALWAYS USE SPACES -- NOT TABS! If you editor-of-choice (or IDE) can't manage
this automatically, please try a new one. If you need help configuring this in
Xamarin Studio or Visual Studio or Code or Rider, please ask.

* Please indent in increments of 2 (spaces). Lots of projects use 4, but that's
just greedy. Not all of use 30-inch monitors.

* Please try to keep lines of code to -- at most -- 80 chars in length (see previous bullet point about monitor size). 
This is, obviously, not always possible (espeically where documentation is concerned), but do make a concerted effort.
If you need advice, please ask.

* If adding a new public-facing module/type/function/whatever, please include:

    * XMLDoc comments (these begin with a triple-back-slash, `///`) -- Note: these may contain XML _OR_ Markdown

    * Tests (unit tests, property tests, performance tests, et cetera)

    * Optionally, narrative-style help docs

##### F#-specific guidelines

For F#-specific guidance, please consult the following references:

* [F# Style Guide](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/)

Additionally, have a look at the existing `fszmq` source code. Examples abound.
As with all things, if you have questions, please ask.

### Library design

fszmq mostly follows the ZeroMQ guidelines for language bindings, and uses an approach similar to many C libraries. 
Specifically, the three core ZeroMQ "concepts" (`Context`, `Socket`, and `Message`) are 
each modeled as a type with a definite lifetime (i.e. must be instantiated and implements `System.IDisposable`),
which wraps a native resource. 
However, all non-lifecycle operations are defined as functions in various modules (e.g. `fszmq.Socket`, `fszmq.Polling`). 
Nearly all of these module functions expect an instance of (at least) one of the core types as input. '
Further, many of the module functions are extensions when used from CLR languages other than F# 
(i.e. they present as member functions on instances of `Context`, `Socket`, or `Message`). 
Finally, many relevant constant values are defined in the `fszmq.ZMQ` module. 
With this core in place, it is possible to add new and different APIs atop the basic library design.

---

Happy Coding!

Have fun. And welcome to the ZeroMQ Community!
