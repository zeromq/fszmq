open System
open System.IO

type System.IO.DirectoryInfo with
  member self.FindFiles(pat) = self.EnumerateFiles(pat)

type System.IO.FileInfo with
  member self.CopyTo(trg:DirectoryInfo) =
    let nm = Path.Combine(trg.FullName,self.Name)
    self.CopyTo(nm,true) |> ignore

let copyAll (trg:DirectoryInfo) = 
  Seq.iter (fun (f:FileInfo) -> f.CopyTo(trg))

let src1,src2,trg =
  let pass = fsi.CommandLineArgs |> Array.findIndex ((=) "--")
  let args = fsi.CommandLineArgs.[(pass + 1)..]
  if args.Length <> 3 then 
    failwith "must provide paths for sources and target"
  DirectoryInfo(args.[0]),
  DirectoryInfo(args.[1]),
  DirectoryInfo(args.[2])

if not <| trg.Exists then 
  trg.Create()
  trg.Refresh()

[ "*.dll";
  "*.pdb";
  "*.xml" ] 
  |> Seq.collect (fun p -> src1.FindFiles(p))
  |> copyAll trg

src2.FindFiles("*.*") |> copyAll trg
