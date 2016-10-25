#if INTERACTIVE
#load "packages/FsLab/FsLab.fsx"
#else
namespace GreyTide
module GreyTide =
#endif

    open System
    open Deedle
    open FSharp.Data
    open XPlot.GoogleCharts
    open XPlot.GoogleCharts.Deedle

    let [<Literal>] statesFile = __SOURCE_DIRECTORY__ + """\v1\States.json"""
    // let [<Literal>] statesFile = """http://greytide.azurewebsites.net/tide/v1/Models/"""
    
    let [<Literal>] modelsFile = __SOURCE_DIRECTORY__ + """\v1\Models.json"""
    //let [<Literal>] modelsFile = http://greytide.azurewebsites.net/tide/v1/Models/

    type States = JsonProvider<statesFile>
    type Models = JsonProvider<modelsFile>
    let states = States.Load(statesFile)
    let models = Models.Load(modelsFile)

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
    //V1 : int * Guid * string * string * (string * int * string * int * Guid (Option(string) * Option(string) * Option(string) * Option(Guid)) []) []) []
    //V2 : int * Guid * string * string * (string * int * string * int * Guid * string []) []) []
    
    //Helper methods
    let inline addDays num (date:DateTime) = num |> float |> date.AddDays 
    let daysAfter date daysToAdd = date |> DateTime.Parse |> addDays daysToAdd
    let toKeyWithValue value key = key,value 
    let keyBeforeToday (date,_) = date < DateTime.Now
    let byName series (name,_) = name
    let byDate series (name,(date,_)) = date
    let inline sumSeriesByPoints (date,total) (_,(_,newvalue:float)) = date,total+(newvalue)
    let byParsedDate series (date,_) = date |> DateTime.Parse
    let sumUpPoints (_:'a) (xs:Series<'b,('c * float)>) : float = xs |> Series.values |> Seq.map (snd) |> Seq.sum 
    let sumModelsByPoints xs = Seq.sumBy (fun (m:Models.Root) -> m.Points) xs

    //Big array of all the days to fill gaps in my data
    let days = 
        daysAfter "1/1/2015" >> toKeyWithValue 0.
        |> Seq.initInfinite  
        |> Seq.takeWhile keyBeforeToday 
        |> Seq.map (fun (d,f) -> d.ToShortDateString(),f) 
        |> Seq.toArray

    //State machine lookup 
    let tryFindState name = 
        states 
        |> Array.collect (fun m -> m.Events) 
        |> Array.map(fun e -> e.Name, e)
        |> Map.ofArray
        |> Map.tryFind name
    let chart = 
        models 
            //Group everything by the thing I'm working on & it's Army
            |> Seq.groupBy (fun model -> model.Faction,model.CurrentState) 
            // Subtotal points by state (primed, painted, done) and it's Army 
            |> Seq.map(fun ((faction,state),models) -> state,faction,sumModelsByPoints models)
            //Convert to a dataframe
            |> Frame.ofValues
            |> Frame.fillMissingWith 0
            |> Chart.Bar 
            |> Chart.WithLegend true 
    let data = // Array of events. (I did X on this day, I did Y on this day)
        models
        |> Array.collect (fun model -> 
            model.States //Collapse all the models, grab their state changes
            |> Array.collect (fun state -> //Convert their state transition names (Paint/Prime/Dislike) to states (Painted,Primed,Complete)
                state.Name
                |> tryFindState 
                |> Option.bind (fun newState -> match newState.To with "Nothing" -> None | _ -> Some newState)
                |> Option.map (fun newState -> newState.To, (state.Date, model.Points)) 
                |> Option.toArray)
            |> Array.sortBy (snd >> fst)
            )
    //Assign values to certain work, based on how hard or intensive, or how much value add
    let mapWork state points = 
        match state, (points|>float) with
        | "Assembled",p -> 0.75 * p
        | ("Primed"|"Varnished"),p -> 0.10 * p
        | "Painted",p -> 2.0 * p
        | "Weathered",p -> 0.25 * p
        | _ -> 0.0

    //Get how much work i've done over time
    let chart'=
        data 
            //Format the data for the chart
            |> Array.map (fun (state,(date,points))-> date.Date.ToShortDateString(), mapWork state points)
            //Add blank days 
            |> Array.append days
            |> Series.ofValues 
            // Running total per day 
            |> Series.groupInto byParsedDate sumUpPoints
            // Skip the first day, as it would count inserting data into the system as work done
            |> Series.filter (fun k _ -> k <> DateTime.Parse("1/1/2015"))
            |> Series.sortByKey
            // Moving average of work
            |> Stats.movingMean 75
            |> Chart.Line

    let results' = 
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
            |> Series.scanValues averageRateOfChange (firstDate,0.)   //Normalized by how long it took
            |> Series.map (fun _ series -> series |> snd)
            |> Stats.expandingSum
        data
        |> Array.map (fun (state,(date,points)) -> state,(date,float points))
        |> Array.sortBy(fun (state,(date,points)) -> date)
        |> Series.ofValues
        |> Series.groupInto byName expandingSumRateOfChange
        //This Gives a grid of States, and a bunch of work on any day
        |> Frame.ofColumns 
        |> Frame.fillMissing Direction.Forward 
        |> Frame.fillMissingWith 0

    let chart'' =
        results' 
        |> Chart.Area  
        |> Chart.WithLegend true
        //Add trendlines and labels
        |> Chart.WithOptions (Options(hAxis=Axis(title="Dates"), vAxis=Axis(title="Points worth of models"), pointSize=1, 
                                    trendlines=(results'.ColumnKeys |> Seq.map (fun k -> Trendline(labelInLegend=k,opacity=0.5,lineWidth=5,color="#C0D9EA")) |> Seq.toArray))) 
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
//Exploring StackOverflow http://evelinag.com/ExploringStackOverflow/#/ - https://youtu.be/qlKZKN7il7c

// Most weekend

//     Minecraft : 1.19
//     LWJGL : 1.12
//     SFML (Simple and Fast Multimedia Library) : 1.06
//     D : 1.04
//     pygame : 1.03
// Most weekday

//     SQL Server Reporting Services 2008, r2 : 0.11
//     Infragistics : 0.13
//     SQL Server Reporting Services 2008 : 0.13
//     Axapta : 0.13
//     DocusignAPI : 0.14
// Functional language ratio (> 1 is weekend, <1 is weekday)
//http://evelinag.com/ExploringStackOverflow/#/0/38

//Star wars http://evelinag.com/blog/2015/12-15-star-wars-social-network/#.WA-N4nqTTOU - https://youtu.be/EI8a6hFFRGQ




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

