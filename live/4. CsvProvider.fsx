#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
let [<Literal>] dataFile = __SOURCE_DIRECTORY__ + """/../data/Data.csv"""
//Create CsvProvider type Person for the data file
//Load the People from the data file
//Get the first Row in the Seq
//lookup some properties of first