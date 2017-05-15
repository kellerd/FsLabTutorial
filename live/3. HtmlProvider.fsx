
#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
let [<Literal>] HtmlFile = __SOURCE_DIRECTORY__ + @"/../data/Comparison_of_programming_languages.html"
//let [<Literal>] HtmlFile = @"https://en.wikipedia.org/wiki/Comparison_of_programming_languages"

//TODO:
//Make a Languages type using HtmlProvider with HtmlFile
//Load the HtmlFile into page
//Look at the General Comparison under Tables in page, get the Rows, store in data
//let data = 
    //page...
    //Filter rows by functional

// let result = 
//     [for r in data -> 
//         r.Language, r.Generic]

// let results' = 
//     query { 
//         for r in data do
//         where (r.Generic = "Yes")
//         select (r.Language,r.Procedural, r.Imperative)
//     } |> Seq.toList


