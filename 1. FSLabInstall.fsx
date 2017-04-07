// Download
//https://github.com/fslaborg/FsLab.Templates/archive/basic.zip

// Open folder in VSCode

// Run package manager
//.paket/paket.exe install
//Run command 
//  > Paket Install

//Run with F#
// Alt + / 
// Select text - Alt + Enter
#load "packages/FsLab/FsLab.fsx"
open Deedle

Seq.init 100 (fun x -> x, x * x) 
|> Seq.filter (fun (_,y) -> y % 3 = 1)
|> Seq.take 10
|> series