#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
let [<Literal>] dataFile = __SOURCE_DIRECTORY__ + @"/../data/v1/Data.csv"
type People = CsvProvider<dataFile>
let people = People.Load(dataFile)

let first = people.Rows |> Seq.head
let x = first.Person_ID 
let y = first.Phone 
let z = first.Email

let newFile = 
    people.Append([People.Row(personId=3244,
                            name="Keller, Dan", 
                            first = "Dan", 
                            last = "Keller",
                            middle = "Allan", 
                            email = "daniel.allan.keller@gmail.com",
                            phone = "333.333.3333", 
                            fax = "333.333.3333", 
                            title = "Developer" )])
                            
newFile.Save(path=__SOURCE_DIRECTORY__ + @"/../data/v3/Data.csv")                      
