(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)

open System
// set up environment
#load "../../docs/content/docs.fs"
open docs
PATH.hijack ()

(*-----------------------------------------------------------------------*)

// load working code
#load "Native.fs"
open fszmq
#load "Constants.fs"
open fszmq
#load "Marshal.fs"
open fszmq
#load "Core.fs"
open fszmq
#load "Message.fs"
open fszmq
open fszmq.Message
#load "Socket.fs"
open fszmq
open fszmq.Socket
#load "Context.fs"
open fszmq
open fszmq.Context
#load "Polling.fs"
open fszmq
open fszmq.Polling
#load "Utilities.fs"
open fszmq
open fszmq.Proxying


(*-----------------------------------------------------------------------*)

#time "on"

printfn "%A" ZMQ.version


(*-----------------------------------------------------------------------*)

// clean up
PATH.release ()
