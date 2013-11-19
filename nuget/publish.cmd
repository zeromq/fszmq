REM -----------------------------------------------------------------------
REM This file is part of fszmq.
REM 
REM fszmq is free software: you can redistribute it and/or modify
REM it under the terms of the GNU Lesser General Public License as published 
REM by the Free Software Foundation, either version 3 of the License, or
REM (at your option) any later version.
REM 
REM fszmq is distributed in the hope that it will be useful,
REM but WITHOUT ANY WARRANTY; without even the implied warranty of
REM MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
REM GNU Lesser General Public License for more details.
REM 
REM You should have received a copy of the GNU Lesser General Public License
REM along with fszmq. If not, see <http://www.gnu.org/licenses/>.
REM 
REM Copyright (c) 2011-2013 Paulmichael Blasucci 
REM -----------------------------------------------------------------------

@for %%f in (..\bin\Release\*.nupkg) do @..\.nuget\NuGet.exe push %%f
