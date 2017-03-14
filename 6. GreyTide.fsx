#load "packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle


let [<Literal>] StatesFile = __SOURCE_DIRECTORY__ + """\data\v1\States.json"""  
let [<Literal>] ModelsFile = __SOURCE_DIRECTORY__ + """\data\v1\Models.json"""  
type States = JsonProvider<StatesFile>
let states = States.Load(StatesFile)
type Models = JsonProvider<ModelsFile>
let models = Models.Load(ModelsFile)

//Helpers
open System
let addDays (date:DateTime)  num = num |> float |> date.AddDays 
let toKeyWithValue value key = key,value 
let beforeToday date = date < DateTime.Now
let byName series (name,_) = name
let byDate series (name,(date,_)) = date
let inline sumSeriesByPoints (date,total) (_,(_,newvalue:float)) = date,total+(newvalue)
let sumUpPoints (_:'a) (xs:Series<'b,('c * float)>) : float = Seq.sumBy snd (xs |> Series.values)
let transitionToEvent name = 
    states |> Array.collect (fun m -> m.Events) |> Array.map(fun e -> e.Name, e.To) |> Map.ofArray |> Map.find name
    |> function "Nothing" -> None | event -> Some event
let transitionValues  (model:Models.Root) (state:Models.State,newState:States.Event) = 
    match newState.To with 
    | "Nothing" -> None 
    | _ -> Some (newState.To, (state.Date.Date, model.Points))

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
    //Initialize with 0.'s
    |> Seq.map (fun date -> date, 0.)
    //Convert to an array
    |> Seq.toArray

 // Array of events. (I did X on this day, I did Y on this day)
 // Event, Date part of the event, How many points
let data =
    models
    |> Array.collect (fun model -> 
        model.States
        |> Array.choose (fun transition -> 
            transition.Name
            |> transitionToEvent  //Present tense to past tense (Paint,Prime,Assemble) to (Painted,Primed,Assembled)
            |> Option.map (fun event -> event,(transition.Date.Date, model.Points)))
            |> Array.sortBy (fun (_,(date,_)) -> date)
    )

//Not all work is created equal
let normalizeWork state points = 
    let weight = 
        match state with
        | "Assembled" -> 0.75 
        | "Primed" | "Varnished" -> 0.10 
        | "Painted" -> 2.0 
        | "Weathered" -> 0.25 
        | _ -> 0.0
    weight * (float points) 

//Take the data, which is (state,(date,points)
data 
    //Format the data for the chart, by mapping the array to  date, normalizeWork state points
    |> Array.map (fun (state,(date,points))-> date, normalizeWork state points)
    //Append days to the array
    |> Array.append days
    //Make a Series of these values
    |> Series.ofValues 
    // Take the Series, group into chunks by the (fun _ -> fst) key, map with sumUpPoints to get a total per day
    |> Series.groupInto (fun _ -> fst) sumUpPoints
    // Skip the first day, as it would count inserting data into the system as work done
    |> Series.filter (fun k _ -> k <> DateTime.Parse("1/1/2015"))
    // Sort the Series by the key
    |> Series.sortByKey
    // Create a moving mean from the stats module, for every 75 days
    |> Stats.movingMean 75
    //Create a line chart
    |> Chart.Line


//Take a series group by date, fold to get a total for the day
let sortByDateAndTotal series = 
    series 
    |> Series.groupBy byDate
    |> Series.map (fun date series -> series.Values |> Seq.fold sumSeriesByPoints (date,0.)) 
    |> Series.sortByKey
//Calculate how much of a jump in work I did
let averageRateOfChange (date,total) (newDate,newValue) =
    let (timespan:TimeSpan) = (newDate-date) 
    newDate, newValue / (max timespan.Days 1 |> float)
//Calculate expanding sum, based on some averate rate of changed
let expandingSumRateOfChange _ series = 
    let firstDate = series |> sortByDateAndTotal |> Series.firstKey
    series 
    |> sortByDateAndTotal
    //Normalized by how long it took
    |> Series.scanValues averageRateOfChange (firstDate,0.)
    |> Series.map (fun _ series -> series |> snd)
    //
    |> Stats.expandingSum

let results = 
    data
    //Cast to float
    |> Array.map (fun (state,(date,points)) -> state,(date,float points))
    //Sort by date
    |> Array.sortBy(fun (state,(date,points)) -> date)
    //Create series
    |> Series.ofValues
    //Group by name, taking the growth of values
    |> Series.groupInto byName expandingSumRateOfChange
    //This Gives a grid of States, and a bunch of work on any day
    |> Frame.ofColumns 
    //Mark in missing so chart isn't messed up
    |> Frame.fillMissing Direction.Forward 
    |> Frame.fillMissingWith 0

//Create some trend lines
let trendlines = [| Trendline(labelInLegend="Painted", color="#FF0000") 
                    Trendline(labelInLegend="Primed", color="#FF0000") 
                    Trendline(labelInLegend="Completed", color="#FF0000")  |]
// Titles & Axis
let options = Options (
                hAxis=Axis(title="Dates"),
                vAxis=Axis(title="Points worth of models"),
                pointSize=1,
                trendlines=trendlines) 

results 
//Only take certain columns
|> Frame.sliceCols ["Painted";"Primed";"Completed"] 
//Make an area chart
|> Chart.Area
//Add trendlines and labels
|> Chart.WithOptions options 