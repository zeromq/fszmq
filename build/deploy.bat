ECHO OFF 

SET SRC=%1
SET TRG=%2

rd /s /q  %TRG%
md        %TRG%

xcopy /r /y %SRC%*.dll %TRG%
xcopy /r /y %SRC%*.pdb %TRG%
xcopy /r /y %SRC%*.xml %TRG%

xcopy /r /y %SRC%..\..\..\..\lib\zeromq-2.1\*.* %TRG%
