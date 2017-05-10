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
#load "../packages/FsLab/FsLab.fsx"
open Deedle
let intToChar x = (int 'A') + x |> char

Seq.init 26 intToChar
|> Seq.indexed
|> series