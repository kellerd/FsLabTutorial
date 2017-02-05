#load "packages/FsLab/FsLab.fsx"
open System
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle


[<Literal>]
let statesFile = __SOURCE_DIRECTORY__ + """\v3\States.json"""
//let statesFile = """http://greytide.azurewebsites.net/tide/v1/Models/"""
[<Literal>]
let modelsFile = __SOURCE_DIRECTORY__ + """\v3\Models.json"""
//let modelsFile = """http://greytide.azurewebsites.net/tide/v1/Models/"""

type States = JsonProvider<statesFile>
type Models = JsonProvider<modelsFile>
let states = States.Load(statesFile)
let models = Models.Load(modelsFile)
let inline addDays num (date:DateTime) = num |> float |> date.AddDays 
let daysAfter date daysToAdd = date |> DateTime.Parse |> addDays daysToAdd
let toKeyWithValue value key = key,value 
let keyBeforeToday (date,_) = date < DateTime.Now
let byName series (name,_) = name
let byDate series (name,(date,_)) = date
let inline sumSeriesByPoints (date,total) (_,(_,newvalue:float)) = date,total+(newvalue)
let byParsedDate series (date,_) = date |> DateTime.Parse

let days = 
    daysAfter "1/1/2015" >> toKeyWithValue 0.
    |> Seq.initInfinite  
    |> Seq.takeWhile keyBeforeToday 
    |> Seq.map (fun (d,f) -> d.ToShortDateString(),f) 
    |> Seq.toArray

let tryFindState name = 
    states 
    |> Array.collect (fun m -> m.Events) 
    |> Array.map(fun e -> e.Name, e)
    |> Map.ofArray
    |> Map.tryFind name

models 
    |> Array.groupBy (fun model -> model.Faction,model.CurrentState) 
    |> Array.map(fun ((faction,state),models) -> state,faction,models) 
    |> Frame.ofValues
    |> Frame.map (fun s f ms -> ms |> Array.sumBy (fun (m:Models.Root) -> m.Points))
    |> Frame.fillMissingWith 0
    |> Chart.Bar 
    |> Chart.WithHeight 1920
    |> Chart.WithWidth 1600
    |> Chart.WithLegend true 


//Get how much work i've done over time

let data = 
    models
    |> Array.collect (fun model -> 
        model.States 
        |> Array.collect (fun state -> 
            state.Name 
            |> tryFindState 
            |> Option.bind (fun newState -> 
                                match newState.To with 
                                | "Nothing" -> None 
                                | _ -> (newState.To, (state.Date, model.Points))
                                       |> Some)
            |> Option.toArray)
        |> Array.sortBy (fun (_,(date,_)) -> date)
        )


//Normalize weightings for different stages of assembly

let normalizeWork state points = 
    let weight = 
        match state with
        | "Assembled" -> 0.75 
        | "Primed" | "Varnished" -> 0.10 
        | "Painted" -> 2.0 
        | "Weathered" -> 0.25 
        | _ -> 0.0
    weight * (float points) 


let sumUpPoints _ = Series.values >> Seq.map (snd) >> Seq.sum

data 
    |> Array.map (fun (state,(date,points))-> date.Date.ToShortDateString(), normalizeWork state points)
    |> Array.append days
    |> Series.ofValues 
    |> Series.groupInto byParsedDate sumUpPoints
    |> Series.filter (fun k _ -> k <> DateTime.Parse("1/1/2015"))
    |> Series.sortByKey
    |> Stats.movingMean 100
    |> Chart.Line


//Get rolling sum of state changes                                    
let results' = 
    let sortByDateAndTotal series = 
     series 
     |> Series.groupBy byDate
     |> Series.map (fun date series -> series.Values |> Seq.fold sumSeriesByPoints (date,0.)) 
     |> Series.sortByKey
    let averageRateOfChange (date,total) (newDate,newValue) =
        let (timespan:TimeSpan) = (newDate-date) 
        newDate, newValue / (max timespan.Days 1 |> float)

    let expandingSumRateOfChange _ series = 
        let firstDate = series |> sortByDateAndTotal |> Series.firstKey
        series 
        |> sortByDateAndTotal
        |> Series.scanValues averageRateOfChange (firstDate,0.)   //Normalized by how long it took
        |> Series.map (fun _ series -> series |> snd)
        |> Stats.expandingSum
    data
    |> Array.map (fun (state,(date,points)) -> state,(date,points|>float))
    |> Series.ofValues
    |> Series.groupInto byName expandingSumRateOfChange
    |> Frame.ofColumns 
    |> Frame.fillMissing Direction.Forward 
    |> Frame.fillMissingWith 0

results' |> Chart.Area
|> Chart.WithOptions (Options(hAxis=Axis(title="Dates"), vAxis=Axis(title="Points worth of models"), pointSize=1
                        ,       trendlines=(results'.ColumnKeys |> Seq.map (fun k -> Trendline(labelInLegend=k,opacity=0.5,lineWidth=5,color="#C0D9EA")) |> Seq.toArray)
                              )) 


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

