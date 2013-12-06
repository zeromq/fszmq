# -------------------------------------------------------------------------
# This file is part of fszmq.
# 
# fszmq is free software: you can redistribute it and/or modify
# it under the terms of the GNU Lesser General Public License as published 
# by the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
# 
# fszmq is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
# GNU Lesser General Public License for more details.
# 
# You should have received a copy of the GNU Lesser General Public License
# along with fszmq. If not, see <http://www.gnu.org/licenses/>.
# 
# Copyright (c) 2011-2013 Paulmichael Blasucci 
# -------------------------------------------------------------------------

param($installPath, $toolsPath, $package, $project)

# get native library
$libzmq = $project.ProjectItems.Item("libzmq.dll")

if ($libzmq -ne $null)
{
  # remove file
  $libzmq.Delete()
}
