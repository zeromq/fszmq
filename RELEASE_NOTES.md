### New in 4.0.0-beta
* Enhanced solution structure (tests, deployment, docs, et cetera)
* Upgraded to latest libzmq.dll

### New in 4.0.1-beta
* Experimental message-passing functions
* Migrated code license to LGPLv3
* Migrated documentation license to MIT/X11

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

### New in 4.0.5 (unreleased)
* updated libzmq to 4.0.5
* Fixed working-directory pathing in `Scratch.fsx` to be properly relative
* Added methods to simplify polling a single socket for input (see tryPollIn,TryGetInput)
* !BREAKING! Removed Timing module
* !BREAKING! Changed poll function to DoPoll when called from other CLR languages
* !BREAKING! Normalized many function names in the Message, Context, and Socket modules
* !BREAKING! Changed pollIO function to PollIO when called from other CLR languages
