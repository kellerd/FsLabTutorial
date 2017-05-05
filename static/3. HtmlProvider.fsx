
#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
let [<Literal>] HtmlFile = __SOURCE_DIRECTORY__ + """/../data/Comparison_of_programming_languages.html"""
//let [<Literal>] HtmlFile = @"https://en.wikipedia.org/wiki/Comparison_of_programming_languages"
type Languages = HtmlProvider<HtmlFile>
let page = Languages.Load(HtmlFile) 
let data = 
    page.Tables.``General comparison``.Rows
    |> Array.filter(fun h -> h.Functional = "Yes")
let result = 
    [for r in data -> 
        r.Language, r.Generic]
let results' = 
    query { 
        for r in data do
        where (r.Generic = "Yes")
        select (r.Language,r.Procedural, r.Imperative)
    } |> Seq.toList


