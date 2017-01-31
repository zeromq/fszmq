### New in 12.2.2 (2017/01/31)
* Version bump (attempting to address NuGet.org issues)

### New in 12.2.1 (2016/07/18)
* FIX (untracked) `Socket.getOptionWithBuffer`, `Socket.getOption` fail when reading boolean options

### New in 12.2.0 (2016/07/11)
* NEW API - safer, more discoverable API for socket options using a mixture of unions, active patterns (thanks to Daniel Fabian)
* socket types (as part of context API) now safer with units-of-measure (thanks to Daniel Fabian)

### New in 12.1.1 (2015/08/21)
* FIX (untracked) - corrected small issue with recieving EINTR during `Context` termination
* FIX for Issue #121 - `Socket.sendAll` treats an empty container as an empty 1-frame message (patched by coconaut)
* FIX for Issue #125 - `Context`, `Socket`, and `Message` no longer raise exceptions during finalization (though they will assert in DEBUG builds)
* FIX for Issue #124 - Improved a few vague error messages
* FIX for Issue #93 - `Z85.encode` and `Z85.decode` no longer fail on empty inputs

### New in 12.1.0 (2015/07/21)
* Added support for `zmq_proxy_steerable`; see the `Proxying` and `ProxyingExtensions` modules
* Switched native handles on `Context`, `Socket`, and `Message` types (back) to public visibility
* Added extension methods to simplify using `Version` from languages other than F#
* Added `Socket.tryRecvInto` (mostly so languages other than F# don't have to acknowledge the `Option<'t>` type)
* Added `Message.configure` function
* Added missing socket options and other constants needed for 4.0.x completeness
* Support for libzmq-4.1.x ... `ZMQ.has` function and `Capabilities` type
* Support for libzmq-4.1.x ... `Message.tryGetMetadata` function (equivalent to `zmq_msg_gets`)
* Support for libzmq-4.1.x ... `Message.tryLoadMetadata` function (for languages where the `Option<'t>` type is uncommon)
* Support for libzmq-4.1.x ... various constants related to socket options, context options, and message options
* Support for libzmq-4.1.x ... addressed errors caused by `zmq_msg_t` allocation size

### New in 12.0.1 (2015/07/01)
* Core types (`Message`, `Socket`, and `Context`) now support referential equality based on underlying native handle
* Calling `ToString()` on core types (`Message`, `Socket`, and `Context`) now includes the value of the native handle
* `Context` is now thread-safe in its managment of attached `Socket` instances
* Added `Message.isMatch` for comparing contents of `Message` instances
* Added operators (|<<) and (>>|) as directinal aliases for `Message.recv`
* !BREAKING! `Message.tryRecv` now returns a boolean rather than an `Option<byte[]>`
* !BREAKING! `Message.recv` now returns Unit rather than an `Option<byte[]>`
* !BREAKING! `Message.tryRecv` and `Message.recv` now take a `Message` instance as an argument  
* !BREAKING! Order of arguments has been reversed on `Message.send` and `Message.sendMore`

### New in 4.0.9-prerelease (2015/05/06)
* Split project into Mac-friendly and Windows-friendly solutions
* Removed the need for compiling with `--define:BSD_EAGAIN` 

### New in 4.0.8 (2015/01/16)
* Fixed bugs related to LINGER being set during `Socket` disposal
* `Context` now tracks and disposes any `Socket` instances associated with it
* Improved `Message` constructor usage from languages other than F#
* !BREAKING! The `Handle` property on `Message`, `Socket`, and `Context` instances is now internal
* Updated bundled libzmq.dll to version 4.0.6

### New in 4.0.7-prerelease (2015/01/16)
* Fixed bugs related to LINGER being set during `Socket` disposal
* `Context` now tracks and disposes any `Socket` instances associated with it
* Improved `Message` constructor usage from languages other than F#
* !BREAKING! The `Handle` property on `Message`, `Socket`, and `Context` instances is now internal

### New in 4.0.6 (2014/06/10)
* Added assembly-level Extension attribute
* Removed unused P/Invoke functions (last vestiges of Timing module)
* Refactored monitoring API to include convenience functions (also extension methods)

### New in 4.0.5 (2014/04/27)
* updated libzmq to 4.0.5
* Fixed working-directory pathing in `Scratch.fsx` to be properly relative
* Added methods to simplify polling a single socket for input (see tryPollIn,TryGetInput)
* !BREAKING! Removed Timing module
* !BREAKING! Changed poll function to DoPoll when called from other CLR languages
* !BREAKING! Normalized many function names in the Message, Context, and Socket modules
* !BREAKING! Changed `pollIO` function to `PollIO` when called from other CLR languages
* !BREAKING! Changed `Proxingy.proxy` to return `unit` 
* Fixed potential memory leak in `Message.tryRecv`

### New in 4.0.4 (Released 2013/12/08)
* Tracked against zeromq4x (stable repo)
* Message type now has a corresponding suite of functions in the Message module
* Added utility functions for timing (see Timing module)
* Added utility functions for Base85 encoding (see Z85 module)
* Added utility functions for CURVE security (see Curve module). WARNING: experimental!
* Added unit tests
* Added API documentation
* Added narrative examples
* Refactored performance tests
* NuGet package now installs either x86 or x64 version of libzmq.dll (based on project configuration)
* NuGet package now has proper uninstaller

### New in 4.0.1-beta
* Experimental message-passing functions
* Migrated code license to LGPLv3
* Migrated documentation license to MIT/X11

### New in 4.0.0-beta
* Enhanced solution structure (tests, deployment, docs, et cetera)
* Upgraded to latest libzmq.dll
