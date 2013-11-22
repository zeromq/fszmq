// --------------------------------------------------------------------------------------
// FAKE build script 
// --------------------------------------------------------------------------------------

#r @"packages/FAKE/tools/FakeLib.dll"
open Fake 
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open System
open System.IO

// --------------------------------------------------------------------------------------
// START TODO: Provide project-specific details below
// --------------------------------------------------------------------------------------

// Information about the project are used
//  - for version and project name in generated AssemblyInfo file
//  - by the generated NuGet package 
//  - to run tests and to publish documentation on GitHub gh-pages
//  - for documentation, you also need to edit info in "docs/tools/generate.fsx"

// The name of the project 
// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
let project = "fszmq"

// Short summary of the project
// (used as description in AssemblyInfo and as a short summary for NuGet package)
let summary = "An LGPLv3-licenced F# binding for the ZeroMQ distributed computing library"

// Longer description of the project
// (used as a description for NuGet package; line breaks are automatically cleaned up)
let description = """An LGPLv3-licensed F# binding for the ØMQ distributed computing library."""

// List of author names (for NuGet package)
let authors = [ "Paulmichael Blasucci" ]
// Tags for your project (for NuGet package)
let tags = "F# fsharp zeromq zmq 0MQ distributed concurrent parallel messaging transport"

// File system information 
// (<solutionFile>.sln and <solutionFile>.Tests.sln are built during the building)
let solutionFile  = "fszmq"
// Pattern specifying assemblies to be tested using NUnit
let testAssemblies = ["tests/*/bin/Release/fszmq*tests*.exe"]

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted 
let gitHome = "https://github.com/pblasucci"
// The name of the project on GitHub
let gitName = "fszmq"

// --------------------------------------------------------------------------------------
// END TODO: The rest of the file includes standard build steps 
// --------------------------------------------------------------------------------------

// Read additional information from the release notes document
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let release = parseReleaseNotes (IO.File.ReadAllLines "RELEASE_NOTES.md")
let license = 
  [|"(* ------------------------------------------------------------------------"
    "This file is part of fszmq."
    ""
    "fszmq is free software: you can redistribute it and/or modify"
    "it under the terms of the GNU Lesser General Public License as published "
    "by the Free Software Foundation, either version 3 of the License, or"
    "(at your option) any later version."
    ""
    "fszmq is distributed in the hope that it will be useful,"
    "but WITHOUT ANY WARRANTY; without even the implied warranty of"
    "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the"
    "GNU Lesser General Public License for more details."
    ""
    "You should have received a copy of the GNU Lesser General Public License"
    "along with fszmq. If not, see <http://www.gnu.org/licenses/>."
    ""
    "Copyright (c) 2011-2013 Paulmichael Blasucci"
    "------------------------------------------------------------------------ *)"|]

// Generate assembly info files with the right version & up-to-date information
Target "AssemblyInfo" (fun _ ->
  let fileName = "src/" + project + "/AssemblyInfo.fs"
  CreateFSharpAssemblyInfoWithConfig fileName
      [ Attribute.Title project
        Attribute.Product project
        Attribute.Description summary
        Attribute.Version release.AssemblyVersion
        Attribute.FileVersion release.AssemblyVersion ] 
        { GenerateClass = false
          UseNamespace  = project }
  // prepend licensing header to start of AssemblyInfo file
  File.WriteAllLines (fileName,Array.append license (File.ReadAllLines fileName))
)

// --------------------------------------------------------------------------------------
// Clean build results & restore NuGet packages

Target "RestorePackages" (fun _ ->
  !! "./**/packages.config"
  |> Seq.iter (RestorePackage (fun p -> { p with ToolPath = "./.nuget/NuGet.exe" }))
)

Target "Clean" (fun _ -> CleanDirs ["bin"; "temp"])

Target "CleanDocs" (fun _ -> CleanDirs ["docs/output"])

// --------------------------------------------------------------------------------------
// Build library & test project

Target "Build" (fun _ ->
  { BaseDirectories = [__SOURCE_DIRECTORY__]
    Includes = [ solutionFile + ".sln" ]
    Excludes = [] } 
  |> MSBuildRelease "" "Rebuild"
  |> ignore
  // .fsproj outputs will automatically wind up in the right locations, 
  // but native libraries need to be moved manually
  CopyDir "bin/zeromq/x86" "lib/zeromq/x86" (fun _ -> true)
  CopyDir "bin/zeromq/x64" "lib/zeromq/x64" (fun _ -> true)
)

// --------------------------------------------------------------------------------------
// Run the unit tests

Target "RunTests" (fun _ ->
  if not <|({ BaseDirectories = [__SOURCE_DIRECTORY__]
              Includes = testAssemblies
              Excludes = [] } 
            |> Seq.map (fun testBlock -> { defaultParams with Program = testBlock })
            |> Seq.map shellExec
            |> Seq.forall ((=) 0))
  then failwith "one or more tests failed"
)

// --------------------------------------------------------------------------------------
// Build a NuGet package

Target "NuGet" (fun _ ->
  // Format the description to fit on a single line (remove \r\n and double-spaces)
  let description = description.Replace("\r", "" )
                               .Replace("\n", "" )
                               .Replace("  ", " ")
  let nugetPath = ".nuget/nuget.exe"
  NuGet (fun p -> 
      { p with   
          Authors = authors
          Project = project
          Summary = summary
          Description = description
          Version = release.NugetVersion
          ReleaseNotes = String.Join(Environment.NewLine, release.Notes)
          Tags = tags
          OutputPath = "bin"
          WorkingDir = "nuget"
          ToolPath = nugetPath
          AccessKey = getBuildParamOrDefault "nugetkey" ""
          Publish = hasBuildParam "nugetkey"
          Dependencies = [] })
      ("nuget/" + project + ".nuspec")
)

// --------------------------------------------------------------------------------------
// Generate the documentation

Target "GenerateDocs" (fun _ ->
  let fsiArgs = if hasBuildParam "Release" then ["--define:RELEASE"] else []
  executeFSIWithArgs "docs/tools" "generate.fsx" fsiArgs [] |> ignore
)

// --------------------------------------------------------------------------------------
// Release Scripts

Target "ReleaseDocs" (fun _ ->
  let ghPages      = "gh-pages"
  let ghPagesLocal = "temp/gh-pages"
  Repository.clone "temp" (gitHome + "/" + gitName + ".git") ghPages
  Branches.checkoutBranch ghPagesLocal ghPages
  CopyRecursive "docs/output" ghPagesLocal true |> printfn "%A"
  CommandHelper.runSimpleGitCommand ghPagesLocal "add ." |> printfn "%s"
  let cmd = sprintf """commit -a -m "Update generated documentation for version %s""" release.NugetVersion
  CommandHelper.runSimpleGitCommand ghPagesLocal cmd |> printfn "%s"
  Branches.push ghPagesLocal
)

Target "Release" DoNothing

// --------------------------------------------------------------------------------------
// Display a list of available targets. Useful if you haven't seen this project in a while

Target "List" (fun _ -> PrintTargets())

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing

"Clean"
  ==> "RestorePackages"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "RunTests"
  ==> "All"

"All" 
  ==> "CleanDocs"
  ==> "GenerateDocs"
//==> "ReleaseDocs"
  ==> "NuGet"
  ==> "Release"

RunTargetOrDefault "All"
