REM -----------------------------------------------------------------------                          
REM Copyright (c) Paulmichael Blasucci.                                        
REM                                                                        
REM This source code is subject to terms and conditions of the Apache  
REM License, Version 2.0. A copy of the license can be found in the    
REM License.html file at the root of this distribution.                                          
REM
REM By using this source code in any fashion, you are agreeing to be bound     
REM by the terms of the Apache License, Version 2.0.                           
REM                                                                        
REM You must not remove this notice, or any other, from this software.         
REM -----------------------------------------------------------------------

REM this script assumes fsi.exe is in the PATH evironment variable

set NUGET="C:\Program Files (x86)\NuGet Bootstrap\nuget.exe"
set ROOT=C:\dev\fs-zmq

fsi deploy.fsx -- %ROOT% %NUGET%
