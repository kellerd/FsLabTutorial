
#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
let [<Literal>] HtmlFile = __SOURCE_DIRECTORY__ + """/../data/Comparison_of_programming_languages.html"""
//let [<Literal>] HtmlFile = @"https://en.wikipedia.org/wiki/Comparison_of_programming_languages"
//Make a Languages type using HtmlProvider with HtmlFile
type Languages = HtmlProvider<HtmlFile>
//Load the HtmlFile into page
let page = Languages.Load(HtmlFile) 
//Look at the General Comparison under Tables in page, get the Rows, store in data
let data = 
    page.Tables.``General comparison``.Rows
    //Filter rows by functional
    |> Array.filter(fun h -> h.Functional = "Yes")
let result = 
    [for r in data -> 
        r.Language, r.Generic]
//Make a query {}
let results' = 
    query { 
        for r in data do
        where (r.Generic = "Yes")
        select (r.Language,r.Procedural, r.Imperative)
    } |> Seq.toList


