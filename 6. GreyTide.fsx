#load "packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

//Helpers
open System
let addDays (date:DateTime)  num = num |> float |> date.AddDays 
let toKeyWithValue value key = key,value 
let beforeToday date = date < DateTime.Now
let byName series (name,_) = name
let byDate series (name,(date,_)) = date
let inline sumSeriesByPoints (date,total) (_,(_,newvalue:float)) = date,total+(newvalue)
let sumUpPoints (_:'a) (xs:Series<'b,('c * float)>) : float = Seq.sumBy snd (xs |> Series.values)
let [<Literal>] StatesFile = __SOURCE_DIRECTORY__ + """\data\v1\States.json"""  
let [<Literal>] ModelsFile = __SOURCE_DIRECTORY__ + """\data\v1\Models.json"""  
type States = JsonProvider<StatesFile>
let states = States.Load(StatesFile)
type Models = JsonProvider<ModelsFile>
let models = Models.Load(ModelsFile)


//What kind of inventory do I have right now.
// Take models
models 
// Group models by Faction,CurrentState
|> Seq.groupBy (fun model -> model.Faction, model.CurrentState) 
// Map groupings of ((faction,state),models) to state, faction, models which are summed by Points
|> Seq.map(fun ((faction,state),models) -> state,faction, models |> Seq.sumBy (fun m -> m.Points) )
//Create a Frame of the values
|> Frame.ofValues
//Take the Frame, Fill missing values with 0
|> Frame.fillMissingWith 0
//Create a Bar Chart
|> Chart.Bar 
// Chart with legend set to true
|> Chart.WithLegend true 

// Step 1, clean up data
// Some blank days, don't want line charts to be all over the place
//For ease, start at 1/1/2015
let days = 
    let firstDate = DateTime.Parse "1/1/2015" 
    //Make an infite sequence using (add days initialized with firstDate)
    Seq.initInfinite (addDays firstDate)
    //Only take values that are before today
    |> Seq.takeWhile beforeToday 
    //Convert to an array
    |> Seq.toArray

