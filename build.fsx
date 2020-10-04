open Fake.DotNet.Testing
(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)

#r "System.Xml.Linq"
#r "paket:
nuget Fake.Api.GitHub
nuget Fake.Core.Environment
nuget Fake.Core.ReleaseNotes
nuget Fake.Core.Target
nuget Fake.Core.Trace
nuget Fake.DotNet.AssemblyInfoFile
nuget Fake.DotNet.Cli
nuget Fake.DotNet.Paket
nuget Fake.IO.FileSystem
nuget Fake.IO.Zip
nuget Fake.Tools.Git //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Tools.Git

Target.initEnvironment()

// open Fake.Git
// open Fake.AssemblyInfoFile
open System
open System.IO

// The name of the project
// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
let project = "fszmq"

// Short summary of the project
// (used as description in AssemblyInfo and as a short summary for NuGet package)
let summary = "An MPLv2-licensed F# binding for the ZeroMQ distributed computing library."

// File system information
let solutionFile  = "fszmq.sln"

// Pattern specifying assemblies to be tested using NUnit
let testAssemblies = "tests/*.tests*/*.tests*.fsproj"

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "zeromq"
let gitHome = "https://github.com/" + gitOwner

// The name of the project on GitHub
let gitName = "fszmq"

// The url for the raw files hosted
let gitRaw = Environment.environVarOrDefault "gitRaw" "https://raw.github.com/zeromq"

// Standard file header
let [<Literal>] HEADER = """This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/."""

// Adapt header to various comment blocks
let newLineChars = Environment.NewLine.ToCharArray ()
let fsHeader =  String.Join ( Environment.NewLine
                            , [| "(* ------------------------------------------------------------------------"
                                 HEADER
                                 "------------------------------------------------------------------------ *)" |])
let csHeader =  String.Join ( Environment.NewLine
                            , HEADER.Split newLineChars
                              |> Array.map (fun l -> "//" + l) )
let vbHeader =  String.Join ( Environment.NewLine
                            , HEADER.Split newLineChars
                              |> Array.map (fun l -> "' " + l) )

// Read additional information from the release notes document
let release = ReleaseNotes.load "RELEASE_NOTES.md"

// Helper active pattern for project types
let (|Fsproj|Csproj|Vbproj|) (projFileName:string) =
  match projFileName with
  | f when f.EndsWith "fsproj"  ->  Fsproj
  | f when f.EndsWith "csproj"  ->  Csproj
  | f when f.EndsWith "vbproj"  ->  Vbproj
  | _ ->  projFileName
          |> sprintf "Project file %s not supported. Unknown project type."
          |> failwith

// Generate assembly info files with the right version & up-to-date information
Target.create "AssemblyInfo" (fun _ ->
  let getAssemblyInfoAttributes projectName =
    [ AssemblyInfo.Title (projectName)
      AssemblyInfo.Product project
      AssemblyInfo.Description summary
      AssemblyInfo.Version release.AssemblyVersion
      AssemblyInfo.FileVersion release.AssemblyVersion ]

  let getProjectDetails (projectPath:string) =
    let projectName = Path.GetFileNameWithoutExtension projectPath
    (projectPath
    ,projectName
    ,Path.GetDirectoryName projectPath
    ,getAssemblyInfoAttributes projectName)

  let writeFile builder attributes header fileName =
    builder fileName attributes
    let before  = File.ReadAllText fileName
    let after   = sprintf "%s%s%s" header Environment.NewLine before
    File.WriteAllText (fileName,after)

  !! "src/**/*.??proj"
  |> Seq.map getProjectDetails
  |> Seq.iter (fun (projFileName, projectName, folderName, attributes) ->
      match projFileName with
      | Fsproj -> writeFile AssemblyInfoFile.createFSharp
                            attributes
                            fsHeader
                            (folderName @@ "AssemblyInfo.fs")
      | Csproj -> writeFile AssemblyInfoFile.createCSharp
                            attributes
                            csHeader
                            ((folderName @@ "Properties") @@ "AssemblyInfo.cs")
      | Vbproj -> writeFile AssemblyInfoFile.createVisualBasic
                            attributes
                            vbHeader
                            ((folderName @@ "My Project") @@ "AssemblyInfo.vb")))

// Copies binaries from default VS location to exepcted bin folder
Target.create "CopyBinaries" (fun _ ->
  // managed libraries
  !! "src/fszmq/bin/Release/netstandard2.0/fszmq.*" |> Shell.copyTo "bin"
  // native dependencies
  !! "lib/zeromq/OSX/**/*.*" |> Shell.copyTo "bin/OSX"
  !! "lib/zeromq/LIN/**/*.*" |> Shell.copyTo "bin/LIN"
  !! "lib/zeromq/WIN/x86/libzmq.*" |> Shell.copyTo "bin/WIN/x86"
  !! "lib/zeromq/WIN/x64/libzmq.*" |> Shell.copyTo "bin/WIN/x64")

// --------------------------------------------------------------------------------------
// Clean build results

Target.create "Clean"      (fun _ -> Shell.cleanDirs ["bin"; "temp"])
Target.create "CleanDocs"  (fun _ -> Shell.cleanDirs ["docs/output"])
Target.create "CleanGuide" (fun _ -> Shell.cleanDirs ["docs/content/zguide"])

// --------------------------------------------------------------------------------------
// Build library & test project

Target.create "Build" (fun _ ->
  DotNet.build (fun p -> { p with Configuration = DotNet.BuildConfiguration.Release })
  |> ignore)

// --------------------------------------------------------------------------------------
// Run the unit tests using test runner

Target.create "RunTests" (fun _ ->
  !! testAssemblies
  |> Seq.iter (fun proj ->
      DotNet.test
        (fun p -> { p with Configuration = DotNet.BuildConfiguration.Release })
        proj))

// --------------------------------------------------------------------------------------
// Build a deployment artifacts

Target.create "Archive" (fun _ ->
  let rootDir = "temp/fszmq-" + release.NugetVersion
  // set up desired file structure
  !! "bin/*.dll" ++ "bin/*.xml"     |> Shell.copy rootDir
  !! "lib/zeromq/OSX/**/*.*"        |> Shell.copy (rootDir + "/OSX")
  !! "lib/zeromq/LIN/**/*.*"        |> Shell.copy (rootDir + "/LIN")
  !! "lib/zeromq/WIN/x86/libzmq.*"  |> Shell.copy (rootDir + "/WIN/x86")
  !! "lib/zeromq/WIN/x64/libzmq.*"  |> Shell.copy (rootDir + "/WIN/x64")
  // compress
  !! (rootDir + "/**/*.*")
    |> Zip.zip rootDir ("bin/fszmq-" + release.NugetVersion + ".zip"))

Target.create "NuGet" (fun _ ->
  Paket.pack(fun p ->
    { p with
        OutputPath = "bin"
        Version = release.NugetVersion
        MinimumFromLockFile = true
        ReleaseNotes = String.toLines release.Notes }))

Target.create "BuildPackage" ignore

// --------------------------------------------------------------------------------------
// Generate the documentation

let generateDocs (refDocs,helpDocs) debug =
  // stage release notes for formatting
  Shell.copyFile "docs/content/release_notes.md" "RELEASE_NOTES.md"
  // configure formatting options
  let args = [ (if not debug then "--define:RELEASE"   else "")
               (if refDocs   then "--define:REFERENCE" else "")
               (if helpDocs  then "--define:HELP"      else "") ]
  // do it!
  let result =
    DotNet.exec
      (fun o -> {o with WorkingDirectory = "docs/tools"})
      "fsi"
      "generate.fsx"
  if result.OK then
    Trace.traceImportant "Help generated"
  else
      Trace.traceError "generating help documentation failed..."
      for x in result.Errors do Trace.traceErrorfn "%s" x

  // clean up
  File.delete "docs/content/release_notes.md"

Target.create "GenerateRefDocs"      (fun _ -> generateDocs (true ,false) false)
Target.create "GenerateHelp"         (fun _ -> generateDocs (false,true ) false)
Target.create "GenerateRefDocsLocal" (fun _ -> generateDocs (true ,false) true )
Target.create "GenerateHelpLocal"    (fun _ -> generateDocs (false,true ) true )

Target.create "GenerateDocs"       ignore
Target.create "GenerateDocsLocal"  ignore

// --------------------------------------------------------------------------------------
// Release Scripts

Target.create "ReleaseDocs" (fun _ ->
  let tempDocsDir = "temp/gh-pages"
  Shell.cleanDir tempDocsDir
  Repository.cloneSingleBranch "" (gitHome + "/" + gitName + ".git") "gh-pages" tempDocsDir

  Shell.copyRecursive "docs/output" tempDocsDir true |> Trace.tracefn "%A"
  Staging.stageAll tempDocsDir
  Commit.exec tempDocsDir (sprintf "Update generated documentation for version %s" release.NugetVersion)
  Branches.push tempDocsDir)

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target.create "All" ignore

"Clean"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "CopyBinaries"
  ==> "RunTests"
  ==> "All"

"All"
  ==> "Archive"
  ==> "NuGet"
  ==> "BuildPackage"

"CopyBinaries"
  ==> "CleanDocs"
  ==> "GenerateHelp"
  ==> "GenerateRefDocs"
  ==> "GenerateDocs"
  =?> ("ReleaseDocs", BuildServer.isLocalBuild)

"CopyBinaries"
  ==> "CleanDocs"
  ==> "GenerateHelpLocal"
  ==> "GenerateRefDocsLocal"
  ==> "GenerateDocsLocal"

Target.runOrDefault "All"
