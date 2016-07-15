#load "packages/FsLab/FsLab.fsx"
open System
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle



type States = JsonProvider<"""http://greytide.azurewebsites.net/tide/v2/States""">
type Models = JsonProvider<"""http://greytide.azurewebsites.net/tide/v2/Models/""">
let states = States.Load("""http://greytide.azurewebsites.net/tide/v2/States/""")
let models = Models.Load("""http://greytide.azurewebsites.net/tide/v2/Models/""")
//State.Events.StateCollectionId changed

// let mapStates = states |> Array.map (fun s -> s.Id, 
//                                               s.Id2,
//                                               s.Name,
//                                               s.Type,
//                                               s.Events 
//                                               |> Array.map (fun (e :States.Event) -> 
//                                                                      e.Name, 
//                                                                      e.Id, 
//                                                                      e.To, 
//                                                                      e.Order, 
//                                                                      e.StateCollectionId, 
//                                                                      e.From
//                                                                      |> Array.map (fun (f : States.From) -> 
//                                                                                             f.Name,
//                                                                                             f.Type,
//                                                                                             f.Id,
//                                                                                             f.StateId)))
//V1 : int * Guid * string * string * (string * int * string * int * Guid (Option(string) * Option(string) * Option(string) * Option(Guid)) []) []) []
//V2 : int * Guid * string * string * (string * int * string * int * Guid * string []) []) []
type Person = JsonProvider<"""[{"name":"Dan", "language":"F#"}]""">
 //                           ,{"name":"Dad"}
let samples = Person.GetSamples()

samples |> Array.map (fun p -> p.Name.Length + p.Language.Length)

models 
|> Array.groupBy (fun model -> model.Faction,model.CurrentState) 
|> Array.map(fun ((faction,state),models) -> state,faction,models) 
|> Frame.ofValues
|> Frame.map (fun s f ms -> ms |> Array.sumBy (fun (m:Models.Root) -> m.Points))
|> Frame.fillMissingWith 0
|> Chart.Bar 
|> Chart.WithLegend true 

let tryFindState name = 
        states 
        |> Array.collect (fun m -> m.Events) 
        |> Array.map(fun e -> e.Name, e)
        |> Map.ofArray
        |> Map.tryFind name
let data = 
    models
    |> Array.collect (fun model -> model.States 
                                    |> Array.collect (fun state -> 
                                                            state.Name 
                                                            |> tryFindState 
                                                            |> Option.bind (fun newState -> match newState.To with "Nothing" -> None | _ -> Some newState)
                                                            |> Option.map (fun newState -> newState.To, (state.Date, model.Points)) 
                                                            |> Option.toArray)
                                    |> Array.sortBy (snd >> fst)
                                    )
//Get how much work i've done over time
let inline addDays num (date:DateTime) = num |> float |> date.AddDays 
let daysAfter date daysToAdd = date |> DateTime.Parse |> addDays daysToAdd
let toKeyWithValue value key = key,value 
let keyBeforeToday (date,_) = date < DateTime.Now
let days = 
    daysAfter "1/1/2015" >> toKeyWithValue 0.
    |> Seq.initInfinite  
    |> Seq.takeWhile keyBeforeToday 
    |> Seq.map (fun (d,f) -> d.ToShortDateString(),f) 
    |> Seq.toArray
    
let mapWork state points = 
    match state, (points|>float) with
     | "Assembled",p -> 0.75 * p
     | ("Primed"|"Varnished"),p -> 0.10 * p
     | "Painted",p -> 2.0 * p
     | "Weathered",p -> 0.25 * p
     | _ -> 0.0

let byParsedDate series (date,_) = date |> DateTime.Parse
let sumUpPoints _ = Series.values >> Seq.map (snd) >> Seq.sum
let results = 
    data 
    |> Array.map (fun (state,(date,points))-> date.Date.ToShortDateString(), mapWork state points)
    |> Array.append days
    |> Series.ofValues 
    |> Series.groupInto byParsedDate sumUpPoints
    |> Series.filter (fun k _ -> k <> DateTime.Parse("1/1/2015"))
    |> Series.sortByKey
    |> Stats.movingMean 75
    |> Chart.Line

let byName series (name,_) = name


//Get rolling sum of state changes                                    
let results' = 
    data
    |> Array.map (fun (state,(date,points)) -> state,(date,points|>float))
    |> Series.ofValues
    |> Series.groupInto byName
        (fun _ series ->  
                            let dateValueSeries = 
                                series 
                                |> Series.groupBy (fun _ (name,(date,points)) -> date) 
                                |> Series.map (fun date series -> series.Values 
                                                                    |> Seq.fold (fun (date,total) (_,(_,newvalue)) -> date,total+(newvalue)) (date,0.)) 
                                |> Series.sortByKey
                            let firstDate = dateValueSeries |> Series.firstKey
                            Series.scanValues (fun (date,total) (newDate,newValue) -> 
                                let (timespan:TimeSpan) = (newDate-date) 
                                newDate, newValue / (max timespan.Days 1 |> float)) (firstDate,0.) dateValueSeries  //Normalized by how long it took
                            |> Series.map (fun _ series -> series |> snd)
                            |> Stats.expandingSum
                        )
    |> Frame.ofColumns |> Frame.fillMissing Direction.Forward |> Frame.fillMissingWith 0
let ChartWithOptions keys = 
    let options = 
        Options(pointSize=1, 
                trendlines=(keys |> Seq.map (fun k -> Trendline(labelInLegend=k,opacity=0.5,lineWidth=5,color="#C0D9EA")) |> Seq.toArray),
                hAxis=Axis(title="Dates"), 
                vAxis=Axis(title="Points worth of models"))
    Chart.WithOptions options     
results' |> Chart.Area  |> Chart.WithLegend true  |> ChartWithOptions results'.ColumnKeys



    // |> Series.map (fun k series -> let x = series |> Stats.sum
    //                                x)
    // |> Stats.movingMean 50
    // |> Series.groupInto 
    //     (fun _ (name,_) -> name) 
    //     (fun _ series -> let x =
    //                         series 
    //                         |> Series.map (fun _ (s,(d,p)) -> series |> snd |> snd  ) 
    //                         |> Stats.movingMean 5
    //                      x
    //                     )
    // |> Frame.ofColumns |> Frame.fillMissing Direction.Forward |> Frame.fillMissingWith 0
//http://fsprojects.github.io/FSharp.Data.TypeProviders/sqldata.html
//http://bluemountaincapital.github.io/FSharpRProvider/mac-and-linux.html
//http://fsprojects.github.io/SQLProvider/
//http://fsprojects.github.io/FSharp.Data.SqlClient/
//http://fsprojects.github.io/DynamicsCRMProvider/



//bug in newer XPLot.Deedle, could do , has to update some references
// Workaround with convert frame to series
// let rows : Series<string,Series<string,int>> = data |> Frame.getRows
// rows |> Series.values |> Chart.Bar |> Chart.WithLabels rows.Keys



//Other versions of getting Data

// let rawData = models 
//                 |> Array.collect (fun m -> m.States 
//                                                 |> Array.map (fun s -> m,s)) 
//                                                 |> Array.sortBy (fun (_,s) -> s.Date)
// let eventMap = 
//     states 
//     |> Array.collect (fun m -> m.Events) 
//     |> Array.map(fun e -> e.Name, e.To)
//     |> Map.ofArray
// let defaultData = eventMap |> Map.toArray 
//                     |> Array.map (fun (_,e) -> 0, e) 
//                     |> Array.distinct
// let barChartData = models 
//                 |> Array.groupBy (fun m-> m.Faction)  
//                 |> Array.map(fun (faction,models) -> 
//                                    faction, models 
//                                    |> Array.map( fun m -> m.Points, m.CurrentState)
//                                    |> Array.append defaultData)
//                 |> Array.map (fun (faction,arr) ->  
//                                      faction, arr 
//                                      |> Array.groupBy snd 
//                                      |> Array.map (fun (state,stats) -> state, stats |> Array.sumBy fst ))
// barChartData 
// |> Array.map snd
// |> Chart.Column
// |> Chart.WithLabels (barChartData |> Array.map fst)

// let data =  
//         models 
//         |> Array.map (fun m -> m.Faction, m.CurrentState, m.Points) 
//         |> Series.ofValues
//         |> Series.groupInto (fun _ (faction,points,state) -> faction) 
//                             (fun faction series -> 
//                                 series 
//                                     |> Series.map (fun key (faction,state,points) -> state,points) 
//                                     |> Series.groupBy (fun key (state,points) -> state)
//                                     |> Series.map (fun key series -> series |> Series.values |> Seq.sumBy snd))

