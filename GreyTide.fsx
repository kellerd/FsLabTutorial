#load "packages/FsLab/FsLab.fsx"

open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

type States = JsonProvider<"""http://greytide.azurewebsites.net/tide/v1/States""">
type Models = JsonProvider<"""http://greytide.azurewebsites.net/tide/v1/Models/""">
Models.GetSamples().[0]
let states = States.Load("""http://greytide.azurewebsites.net/tide/States/v1/Models/""")
let models = Models.Load("""http://greytide.azurewebsites.net/tide/Models/v1/Models/""")
//State.Events.StateCollectionId changed

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

type Person = JsonProvider<"""[{"name":"Dan", "language":"F#"}]""">
 //                           ,{"name":"Dad"}
let samples = Person.GetSamples()

samples |> Array.map (fun p -> p.Name.Length + p.Language.Length)


models 
|> Array.groupBy (fun model -> model.Faction,model.CurrentState) 
|> Array.map(fun ((faction,state),models) -> faction,state,models) 
|> Frame.ofValues
|> Frame.map (fun f s ms -> ms |> Array.sumBy (fun (m:Models.Root) -> m.Points))
|> Frame.fillMissingWith 0
|> Chart.Bar 
|> Chart.WithLegend true 



//

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

