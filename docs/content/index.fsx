(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

type ENV = System.Environment

ENV.CurrentDirectory <- sprintf "%s../../bin/zeromq/%s" 
                                __SOURCE_DIRECTORY__ 
                                (if ENV.Is64BitProcess then "x64" else "x86")

(**
F# Project Scaffold
===================

Documentation

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The F# ProjectTemplate library can be <a href="https://nuget.org/packages/fszmq">installed from NuGet</a>:
      <pre>PM> Install-Package fszmq</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Example
-------

This example demonstrates using a function defined in this sample library.

*)

#r "fszmq.dll"
open fszmq

printfn "%A" ZMQ.version

(**
Some more info

Samples & documentation
-----------------------

The library comes with comprehensible documentation. 
It can include a tutorials automatically generated from `*.fsx` files in [the content folder][content]. 
The API reference is automatically generated from Markdown comments in the library implementation.

 * [Tutorial](tutorial.html) contains a further explanation of this sample library.

 * [API Reference](reference/index.html) contains automatically generated documentation for 
   all public types, modules, and functions in the library. 
   This includes additional brief samples on using most of the functions.
 
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
*)
