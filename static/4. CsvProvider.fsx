#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
let [<Literal>] dataFile = __SOURCE_DIRECTORY__ + """/../data/Data.csv"""
type People = CsvProvider<dataFile>
let people = People.Load(dataFile)
let first = people.Rows |> Seq.head
let x = first.Person_ID 
let y = first.Phone 
let z = first.Email
