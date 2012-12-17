# -------------------------------------------------------------------------                                                              
# Copyright (c) Paulmichael Blasucci.                                        
#                                                                            
# This source code is subject to terms and conditions of the Apache  
# License, Version 2.0. A copy of the license can be found in the    
# License.html file at the root of this distribution.                                          
#                                                                            
# By using this source code in any fashion, you are agreeing to be bound     
# by the terms of the Apache License, Version 2.0.                           
#                                                                            
# You must not remove this notice, or any other, from this software.         
# -------------------------------------------------------------------------
param($installPath, $toolsPath, $package, $project)

# get reference to native library
$libzmq = $project.ProjectItems.Item("libzmq.dll")

# set Build Action to None
$buildAction = $libzmq.Properties.Item("BuildAction")
$buildAction.Value = 0

# set Copy to Output Directy to Copy if newer
$copyToOutput = $libzmq.Properties.Item("CopyToOutputDirectory")
$copyToOutput.Value = 2
