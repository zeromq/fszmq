(* ------------------------------------------------------------------------
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
------------------------------------------------------------------------ *)
#if INTERACTIVE
open System
#I "../../packages/ExtCore.0.8.36/lib/net40"
#I "../../packages/FsCheck.0.9.2.0/lib/net40-client"
#I "../../packages/FsUnit.1.2.1.0/Lib/Net40"
#I "../../packages/NUnit.2.6.3/lib"
#I "./bin/Debug"
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/bin/Debug"
printfn "CurrentDirectory = %s" Environment.CurrentDirectory
#r "ExtCore.dll"
#r "FsCheck.dll"
#r "FsUnit.NUnit.dll"
#r "fszmq.dll"
#r "nunit.framework.dll"
#else
namespace fszmq.tests
#endif

open FsUnit
open fszmq
open NUnit.Framework

[<AutoOpen;TestFixture>]
module MessageTest = 
  begin (* TODO: ??? *) end
  