#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
//Make type from sample """{"name":"Dan", "language":"F#"}"""
type Person = JsonProvider<"""[{"name":"Dan", "language":"F#"},{"name":"Brian"}]""">
//Get sample from type
let samples = Person.GetSamples()
//Take sample, send to lambda to get Name and Language
//Change to array in Sample given, add new entry
//Fix samples
//Fix person 
let person = samples |> Seq.head |> fun p -> p.Name, p.Language

//let [<Literal>] StatesFile = """http://greytide.azurewebsites.net/tide/v3/States"""
let [<Literal>] StatesFile = __SOURCE_DIRECTORY__ + """/../data/v1/States.json"""  
type States = JsonProvider<StatesFile>
let states = States.Load(StatesFile)
//Change version to V2
//Comment out missing elements
//Change to azure V2
//Change to azure V3
let mapStates = states |> Array.map (fun s -> s.Id, 
                                              s.Id2,
                                              s.Name,
                                              s.Type,
                                              s.Events 
                                              |> Array.map (fun (e :States.Event) -> 
                                                                     e.Name, 
                                                                     e.Id, 
                                                                     e.To, 
                                                                     e.Order, 
                                                                     e.StateCollectionId, 
                                                                     e.From
                                                                     |> Array.map (fun (f : States.From) -> 
                                                                                            f.Name,
                                                                                            f.Type,
                                                                                            f.Id,
                                                                                            f.StateId)))
//V1 : int * Guid * string * string * (string * int * string * int * Guid (Option(string) * Option(string) * Option(string) * Option(Guid)) []) []) []
//V2 : int * Guid * string * string * (string * int * string * int * Guid * string []) []) []

