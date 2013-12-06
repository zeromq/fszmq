(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

type ENV = System.Environment

let zmqVersion = if ENV.Is64BitProcess then "x64" else "x86"
ENV.CurrentDirectory <- sprintf "%s../../../bin/zeromq/%s" __SOURCE_DIRECTORY__ zmqVersion

(**
fszmq
===================

fszmq is an LGPLv3-licensed F# binding for the ZeroMQ  distributed computing library. 
It provides a complete binding to versions 2.1.x, 3.2.x, and 4.0.x of ZeroMQ 
(Note: each binding is a separate branch in git, as there are some non-compatible differences). 
This library is primarily designed to be consumed from F#. However, where possible, the library has been designed 
to appear "friendly" when consumed by other CLR languages (C#, et aliam).

<div class="row">
  <div class="span2">
    <img src="img/fszmq_logo.png" alt="fszmq" style="height:137px;margin:1.4em 0 0 2em;" />
  </div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The fszmq library can be <a href="https://www.nuget.org/packages/fszmq">installed from NuGet</a>:
      <pre>PM> Install-Package fszmq</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Getting Started
-------

The simplest thing we can do with fszmq is print the ZeroMQ version. That is, we can display the version, 
which is not fixed at compile-time, of the native library (i.e. libzmq) against which fszmq.dll is current bound.
_(Note: the version of fszmq.dll, itself, may be retrieved via the normal mechanisms for CLR libraries.)_

*)
#r "fszmq.dll"
open fszmq

printfn "libzmq version: %A" ZMQ.version

(**
Running this code should produce something like the following (the actual numbers may be different on your machine):

<table class="pre"><tbody><tr><td class="lines"><pre class="fssnip"><span class="l"> 1: </span>
<span class="l"> 2: </span>
<span class="l"> 3: </span></pre>
</td>
<td class="snippet"><pre class="fssnip"><span class="i">&gt;</span>
<span class="i">libzmq version: 4.0.4</span>
<span class="i">&gt;</span></pre>
</td>
</tr>
</tbody></table>
*)

(**
Samples & Documentation
-----------------------

This library features API documentation, and an ever-growing collection of samples. Most of these narrative-style examples
are also executable F# scripts, and may be found in [the content folder][content] (Hint: look for files ending in `.fsx`).

In particular, please review:

 * [Hello, World](tutorial.html) ... contains a more complete, but simplistic, example of fszmq in action.

 * [API Reference](reference/index.html) ... contains automatically generated documentation for all types, modules, 
   and functions in the library. This includes additional brief samples on using some of the functionality.

Additionally, general information about ZeroMQ maybe found at [http://zero.mq](http://zeromq.org). 
And [the zguide][zguide] is a well-written, exhaustive explanation of the library.
While the zguide features examples in many languages, there are [F#-specific zguide code samples](http://pblasucci.github.io/fszmq).
 
Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding new public API, please also 
consider adding [samples][content] that can be turned into a documentation. You might
also want to read [library design notes][readme] to understand how it works.

The fszmq library is available under the GNU LESSER GENERAL PUBLIC LICENSE, version 3, 
which allows modification and redistribution for non-commercial purposes. 
Additionally, redistribution is allowed for commercial purposes. 
For more information see the [license file][license] in the GitHub repository. 

The documentation accompanying fszmq, and any sample code contained therein, is available under
the MIT LICENSE which allows modification and reuse for both commercial non-commercial purposes.
For more information see the [documentation license file][docslicense] in the GitHub repository. 

  [content]: https://github.com/pblasucci/fszmq/tree/master/docs/content
  [gh]: https://github.com/pblasucci/fszmq
  [issues]: https://github.com/pblasucci/fszmq/issues
  [readme]: https://github.com/pblasucci/fszmq/blob/master/README.md
  [license]: https://github.com/pblasucci/fszmq/blob/master/COPYING.lesser
  [docslicense]: https://github.com/pblasucci/fszmq/blob/master/docs/files/LICENSE.txt
  [zguide]: http://zguide.zeromq.org/page:all
*)
