// Download
//https://github.com/fslaborg/FsLab.Templates/archive/basic.zip

// Open folder in VSCode

// Run package manager
//.paket/paket.exe install
// Ionide extension :> Paket Install

//Run with F#
// Alt + / 
// Select text - Alt + Enter
#load "packages/FsLab/FsLab.fsx"
open Deedle

Seq.initInfinite (fun x -> x,x) 
|> Seq.take 100 
|> series