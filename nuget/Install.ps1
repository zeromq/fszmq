param($installPath, $toolsPath, $package, $project)

# get reference to native library
$libzmq = $project.ProjectItems.Item("libzmq.dll")

# set Build Action to None
$buildAction = $libzmq.Properties.Item("BuildAction")
$buildAction.Value = 0

# set Copy to Output Directy to Copy if newer
$copyToOutput = $libzmq.Properties.Item("CopyToOutputDirectory")
$copyToOutput.Value = 2
