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

It provides a stable, well-used, feature-complete binidng to 95% of the ZeroMQ library.
The remaining feature-bindings are _experimentally_ complete. 
This binding is primarily designed to be consumed from F#. 
However, where possible, the library has been designed to appear "friendly" when consumed by other CLR (ie: .NET, Mono) languages (C#, et aliam).


At this point in time, fszmq has been tested against the CLR on the following platform/architecture/runtime combinations:
* 32-bit Windows XP (running against .NET)
* 64-bit Windows Server 2008 R2 (running against .NET)
* 32-bit and 64-bit Windows 7 (running against .NET and/or Mono)
* 32-bit and 64-bit Windows 8 (running against .NET and/or Mono)
* 32-bit and 64-bit OS X Snow Leopard (running agains Mono)

Other platform/architecture/runtime combinations will be tested in due course.

---------------------------------------------------------------------------

#### A note about versions

The master branch of fszmq is usually tracked against the latest stable branch of ZeroMQ.
Separate (inactive) branches are created for major changes, and usually align with stable branches of ZeroMQ.

_TODO: provide further details about the versioning process_

---------------------------------------------------------------------------

_Please note: a full testing suite is under development, but is (woefully) incomplete at this time._

_Please note: comprehensive documentation is under development, but is incomplete at this time._

---------------------------------------------------------------------------

Issues, questions, and concerns may be directed the the [Issue Tracker](http://github.com/pblasucci/fszmq/issues).

More information about ZeroMQ is available at http://zero.mq.

---------------------------------------------------------------------------

###### Copyright &#169; 2011-2013 Paulmichael Blasucci. This project is released under the LGPL (v3) [license](COPYING.lesser).
