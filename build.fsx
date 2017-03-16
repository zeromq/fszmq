(* ------------------------------------------------------------------------
This file is part of fszmq.

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
------------------------------------------------------------------------ *)

#r "System.Xml.Linq"
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open System
open System.IO

let notWin = isUnix || isLinux || isMacOS

// The name of the project
// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
let project = "fszmq"

// Short summary of the project
// (used as description in AssemblyInfo and as a short summary for NuGet package)
let summary = "An MPLv2-licensed F# binding for the ZeroMQ distributed computing library."

// File system information
let solutionFile  = sprintf "fszmq-%s.sln" (if notWin then "osx" else "win")

// Pattern specifying assemblies to be tested using NUnit
let testAssemblies = "tests/**/bin/Release/*tests*.dll"

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "zeromq"
let gitHome = "https://github.com/" + gitOwner

// The name of the project on GitHub
let gitName = "fszmq"

// The url for the raw files hosted
let gitRaw = environVarOrDefault "gitRaw" "https://raw.github.com/zeromq"

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
let release = LoadReleaseNotes "RELEASE_NOTES.md"

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
Target "AssemblyInfo" (fun _ ->
  let getAssemblyInfoAttributes projectName =
    [ Attribute.Title (projectName)
      Attribute.Product project
      Attribute.Description summary
      Attribute.Version release.AssemblyVersion
      Attribute.FileVersion release.AssemblyVersion ]

  let getProjectDetails projectPath =
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
      | Fsproj -> writeFile CreateFSharpAssemblyInfo
                            attributes
                            fsHeader
                            (folderName @@ "AssemblyInfo.fs")
      | Csproj -> writeFile CreateCSharpAssemblyInfo
                            attributes
                            csHeader
                            ((folderName @@ "Properties") @@ "AssemblyInfo.cs")
      | Vbproj -> writeFile CreateVisualBasicAssemblyInfo
                            attributes
                            vbHeader
                            ((folderName @@ "My Project") @@ "AssemblyInfo.vb")))

// Copies binaries from default VS location to exepcted bin folder
Target "CopyBinaries" (fun _ ->
  // managed libraries
  !! "src/fszmq/bin/Release/fszmq.*"
    // support tooling
    ++ "tests/**/bin/Release/*.exe" |> CopyTo "bin"
  // native dependencies
  if notWin
    then  !! "lib/zeromq/OSX/**/*.*" |> CopyTo "bin"
    else  !! "lib/zeromq/WIN/x86/libzmq.*" |> CopyTo "bin/x86"
          !! "lib/zeromq/WIN/x64/libzmq.*" |> CopyTo "bin/x64")

// --------------------------------------------------------------------------------------
// Clean build results

Target "Clean"      (fun _ -> CleanDirs ["bin"; "temp"])
Target "CleanDocs"  (fun _ -> CleanDirs ["docs/output"])
Target "CleanGuide" (fun _ -> CleanDirs ["docs/content/zguide"])

// --------------------------------------------------------------------------------------
// Build library & test project

Target "Build" (fun _ ->
  !! solutionFile
  |> MSBuildRelease "" "Rebuild"
  |> ignore)

// --------------------------------------------------------------------------------------
// Run the unit tests using test runner

Target "RunTests" (fun _ ->
  !! testAssemblies
  |> NUnit (fun p ->
      { p with
          DisableShadowCopy = true
          ToolName          = if notWin
                                then p.ToolName
                                else "nunit-console-x86.exe"
          Framework         = if notWin
                                then p.Framework
                                else "net-4.0"
          TimeOut           = TimeSpan.FromMinutes 20.
          OutputFile        = "tests/TestResults.xml" }))

// --------------------------------------------------------------------------------------
// Build a deployment artifacts

Target "Archive" (fun _ ->
  let rootDir = "temp/fszmq-" + release.NugetVersion
  let winDir  = rootDir + "/WIN"
  // set up desired file structure
  !! "bin/*.dll"
    ++ "bin/*.xml" 
    ++ "bin/*.exe" 
    -- "bin/*.compat.*"             |> Copy rootDir 
  !! "lib/zeromq/OSX/**/*.*"        |> Copy (rootDir + "/OSX")
  !! "lib/zeromq/WIN/x86/libzmq.*"  |> Copy (winDir + "/x86")
  !! "lib/zeromq/WIN/x64/libzmq.*"  |> Copy (winDir + "/x64")
  // compress
  !! (rootDir + "/**/*.*") 
    |> Zip rootDir ("bin/fszmq-" + release.NugetVersion + ".zip"))

Target "NuGet" (fun _ ->
  Paket.Pack(fun p ->
    { p with
        OutputPath = "bin"
        Version = release.NugetVersion
        MinimumFromLockFile = true
        ReleaseNotes = toLines release.Notes}))

Target "BuildPackage" DoNothing

// --------------------------------------------------------------------------------------
// Generate the documentation

let generateDocs (refDocs,helpDocs) debug =
  // stage release notes for formatting
  CopyFile "docs/content/release_notes.md" "RELEASE_NOTES.md"
  // configure formatting options
  let args = [ (if not debug then "--define:RELEASE"   else "")
               (if refDocs   then "--define:REFERENCE" else "")
               (if helpDocs  then "--define:HELP"      else "") ]
  // do it!
  if executeFSIWithArgs "docs/tools" "generate.fsx" args []
    then  traceImportant "Help generated"
    else  traceImportant "generating help documentation failed"
  // clean up
  DeleteFile "docs/content/release_notes.md"

Target "GenerateRefDocs"      (fun _ -> generateDocs (true ,false) false)
Target "GenerateHelp"         (fun _ -> generateDocs (false,true ) false)
Target "GenerateRefDocsLocal" (fun _ -> generateDocs (true ,false) true )
Target "GenerateHelpLocal"    (fun _ -> generateDocs (false,true ) true )

Target "GenerateDocs"       DoNothing
Target "GenerateDocsLocal"  DoNothing

// --------------------------------------------------------------------------------------
// Release Scripts

Target "ReleaseDocs" (fun _ ->
  let tempDocsDir = "temp/gh-pages"
  CleanDir tempDocsDir
  Repository.cloneSingleBranch "" (gitHome + "/" + gitName + ".git") "gh-pages" tempDocsDir

  CopyRecursive "docs/output" tempDocsDir true |> tracefn "%A"
  StageAll tempDocsDir
  Git.Commit.Commit tempDocsDir (sprintf "Update generated documentation for version %s" release.NugetVersion)
  Branches.push tempDocsDir)

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing

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
  =?> ("ReleaseDocs",isLocalBuild)

"CopyBinaries"
  ==> "CleanDocs"
  ==> "GenerateHelpLocal"
  ==> "GenerateRefDocsLocal"
  ==> "GenerateDocsLocal"

RunTargetOrDefault "All"
