#load "../packages/FsLab/FsLab.fsx"
#load "Helpers.fsx"
open Helpers
open System
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle
//let [<Literal>] StatesFile = """http://greytide.azurewebsites.net/tide/v3/States/"""
//let [<Literal>] ModelsFile = """http://greytide.azurewebsites.net/tide/v3/Models/"""
let [<Literal>] StatesFile = __SOURCE_DIRECTORY__ + """/../data/v1/States.json"""  
let [<Literal>] ModelsFile = __SOURCE_DIRECTORY__ + """/../data/v1/Models.json"""  
type States = JsonProvider<StatesFile>
let states = States.Load(StatesFile)
type Models = JsonProvider<ModelsFile>
let models = Models.Load(ModelsFile)
//More helpers
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
|> Seq.groupBy (fun model -> model.Faction, model.CurrentState) 
|> Seq.map(fun ((faction,state),models) -> state,faction, models |> Seq.sumBy (fun m -> m.Points) )
|> Frame.ofValues
|> Frame.fillMissingWith 0
|> Chart.Bar 
|> Chart.WithLegend true 

// Step 1, clean up data
//For ease, start at 1/1/2015
let days = 
    let firstDate = DateTime.Parse "1/1/2015" 
    Seq.initInfinite (addDays firstDate)
    |> Seq.takeWhile beforeToday 
    |> Seq.map (fun date -> date, 0.)
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

//Try to get a plot of how much work is done in certain periods. 
//Are some months better?
//Take the data, which is (state,(date,points)
let firstDate = DateTime.Parse("1/1/2015")
data 
    |> Array.map (fun (state,(date,points))-> date, normalizeWork state points)
    |> Array.append days
    |> Series.ofValues 
    |> Series.groupInto (fun _ -> fst) sumUpPoints   // map with sumUpPoints to get a total per day
    |> Series.filter (fun k _ -> k <> firstDate)
    |> Series.sortByKey
    |> Stats.movingMean 75  // Rolling average of work per 75 days
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
    |> Series.scanValues averageRateOfChange (firstDate,0.)  //Normalized by how long it took
    |> Series.map (fun _ series -> series |> snd)
    |> Stats.expandingSum    //Cumulative adding of columns

//How much culminated work that is in each state 
let results = 
    data
    |> Array.map (fun (state,(date,points)) -> state,(date,float points))
    |> Array.sortBy(fun (state,(date,points)) -> date)
    |> Series.ofValues
    |> Series.groupInto byName expandingSumRateOfChange       //Group and aggregate into, by name,  expandingSum
    |> Frame.ofColumns 
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
|> Frame.sliceCols ["Painted";"Primed";"Completed"] 
|> Chart.Area
|> Chart.WithOptions options 

//Swap with v2,v3 or azure