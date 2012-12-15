REM this script assumes fsi.exe is in the PATH evironment variable

set NUGET="C:\Program Files (x86)\NuGet Bootstrap\nuget.exe"
set ROOT=C:\working\fs-zmq

fsi deploy.fsx -- %ROOT% %NUGET%
