open System

#r "System.IO.Compression"
#r "System.IO.Compression.FileSystem.dll"
open System.IO
open System.IO.Compression

type System.IO.DirectoryInfo with
  member D.MakePath(fileName)       = Path.Combine(D.FullName,fileName)
  member D.MakePath(info:FileInfo)  = D.MakePath(info.Name) 
  
type System.IO.FileInfo with
  member F.CopyTo(info:DirectoryInfo) = F.CopyTo(info.MakePath(F))

// get input arguments
let rootDir,nuget = 
  let emdash  = fsi.CommandLineArgs |> Array.findIndex ((=) "--")
  fsi.CommandLineArgs.[emdash + 1],fsi.CommandLineArgs.[emdash + 2]

// pathing information
let [<Literal>] GITIGNORE = @".gitignore"
let [<Literal>] ARCHIVEXT = @".zip"
let [<Literal>] NUGETSPEC = @"fszmq.nuspec"
let [<Literal>] NUPACKEXT = @".nupkg"
let [<Literal>] ZEROMQLIB = @"libzmq.dll"
let [<Literal>] FSZMQEXTS = @"fszmq.*"

let libDir = DirectoryInfo(Path.Combine(rootDir,"zeromq"))
let pkgDir = DirectoryInfo(Path.Combine(rootDir,"nuget" ))
let binDir = DirectoryInfo(Path.Combine(rootDir,"bin"   ))
let outDir = DirectoryInfo(Path.Combine(rootDir,"deploy"))

// remove existing artifacts from deployment folder
outDir.GetDirectories() 
  |> Array.iter (fun d -> d.Delete(true))

outDir.GetFiles() 
  |> Array.filter (fun f -> f.Name <> GITIGNORE)
  |> Array.iter   (fun f -> f.Delete())
                                            
// copy intermediate files to deployment folder
binDir.GetFiles(FSZMQEXTS) 
  |> Array.append (libDir.GetFiles ZEROMQLIB)
  |> Array.iter   (fun f -> f.CopyTo(outDir) |> ignore)

// compile nuget package
open System.Diagnostics

let package() = 
  let nugetArgs = 
    sprintf "pack %s -OutputDirectory %s -NonInteractive -Verbosity quiet"
            (pkgDir.MakePath NUGETSPEC)
            (outDir.FullName          )
  let startInfo = ProcessStartInfo(nuget,nugetArgs,UseShellExecute=false)
  use nugetProc = Process.Start(startInfo)
  nugetProc.WaitForExit()    

package()
                                                                                                       
// compress intermediate files into archive
let archive() =
  let archiveName = outDir.GetFiles("*" + NUPACKEXT)
                    |> Array.map (fun f -> f.FullName.Replace(NUPACKEXT
                                                             ,ARCHIVEXT))
                    |> Seq.head
  let sourceFiles = outDir.GetFiles()
                    |> Array.filter (fun f -> f.Extension <> GITIGNORE
                                           && f.Extension <> NUPACKEXT
                                           && f.Extension <> ARCHIVEXT)
  use zip = ZipFile.Open(archiveName,ZipArchiveMode.Update)
  for file in sourceFiles do 
    zip.CreateEntryFromFile(file.FullName,file.Name)  |> ignore

archive()
                     
// remove intermediate files from deployment folder
outDir.GetFiles() 
  |> Array.filter (fun f -> f.Extension <> GITIGNORE
                         && f.Extension <> NUPACKEXT
                         && f.Extension <> ARCHIVEXT)
  |> Array.iter   (fun f -> f.Delete())
