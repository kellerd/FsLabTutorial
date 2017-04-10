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

//TODO:
// Take models
// Group models by Faction,CurrentState
// Map groupings of ((faction,state),models) to state, faction, models which are summed by Points
//Create a Frame of the values
//Take the Frame, Fill missing values with 0
//Create a Bar Chart
// Chart with legend set to true



// Step 1, clean up data
// Some blank days, don't want line charts to be all over the place
//For ease, start at 1/1/2015
let firstDate = DateTime.Parse "1/1/2015" 
let days = 
    //TODO:
    //Make an infite sequence using (addDays initialized with firstDate)
    //Only take values that are before today
    //Initialize with 0.'s
    //Convert to an array

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
//Take the data, which is (state,(date,points))
data 
    //TODO:
    //Format the data for the chart, by mapping the array to  date, normalizeWork state points
    //Append days to the array
    //Make a Series of these values
    // Take the Series, group into chunks by the (fun _ -> fst) key, map with sumUpPoints to get a total per day
    // Filter out the Series where the key <> firstDate , as it would count inserting data into the system as work done
    // Sort the Series by the key
    // Create a moving mean from the stats module, for every 75 days
    //Create a line chart

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
    //Only take the point values, not the floats
    |> Series.map (fun _ series -> series |> snd)
    //Cumulative adding of columns
    |> Stats.expandingSum

//How much culminated work that is in each state 
let results = 
    data
    //TODO:
    //Map the array of (state,(date,points)) and cast points to float
    //Sort by date
    //Create series of the values
    //Group and aggregate into, byName,  expandingSumRateOfChange
    //Create a Frame of the columns
    //Fill missing values in the forward direction
    //Fill missing values with 0

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

                
//results
results 
//TODO:
//Take a slice of the Frame columns "Painted";"Primed";"Completed"
//Make an Area Chart
//Add Chart options

//Swap with v2,v3 or azure