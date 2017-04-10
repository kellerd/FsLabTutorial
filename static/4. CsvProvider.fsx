#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
let [<Literal>] dataFile = __SOURCE_DIRECTORY__ + """/../data/Data.csv"""
//Create CsvProvider type Person for the data file
type People = CsvProvider<dataFile>
//Load the People from the data file
let people = People.Load(dataFile)
//Get the first one
let first = people.Rows |> Seq.head
//lookup some properties of first
let x = first.Person_ID 
let y = first.Phone 
let z = first.Email
