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

let square x = x * x
let cube x = x * x * x

let squares = 
    Seq.init 20 square
    |> Seq.indexed
    |> series
let cubes =     
    List.init 20 cube
    |> List.indexed
    |> List.skip 5
    |> List.take 10
    |> series

let result = 
    [ "Squares", squares; 
      "Cubes", cubes ] 
    |> frame