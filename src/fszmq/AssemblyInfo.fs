(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)
namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("fszmq")>]
[<assembly: AssemblyProductAttribute("fszmq")>]
[<assembly: AssemblyDescriptionAttribute("An MPLv2-licensed F# binding for the ZeroMQ distributed computing library.")>]
[<assembly: AssemblyVersionAttribute("12.1.1")>]
[<assembly: AssemblyFileVersionAttribute("12.1.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "12.1.1"
