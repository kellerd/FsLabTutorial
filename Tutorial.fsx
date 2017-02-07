// Before running any code, invoke Paket to get the dependencies. 
//
// You can either build the project (Ctrl + Alt + B in VS) or run 
// '.paket/paket.bootstrap.exe' and then '.paket/paket.exe install'
// (if you are on a Mac or Linux, run the 'exe' files using 'mono')
//
// Once you have packages, use Alt+Enter (in VS) or Ctrl+Enter to
// run the following in F# Interactive. You can ignore the project
// (running it doesn't do anything, it just contains this script)
#load "packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

// Connect to the WorldBank and access indicators EU and CZ
// Try changing the code to look at stats for your country!
let wb = WorldBankData.GetDataContext()
let cz = wb.Countries.``Czech Republic``.Indicators
let eu = wb.Countries.``European Union``.Indicators

// Use Deedle to get time-series with school enrollment data
let czschool = series cz.``Gross enrolment ratio, tertiary, both sexes (%)``
let euschool = series eu.``Gross enrolment ratio, tertiary, both sexes (%)``
// Get 5 years with the largest difference between EU and CZ

let chart = 
    abs (czschool - euschool)
    |> Series.sort
    |> Series.rev
    |> Series.take 5

// Plot a line chart comparing the two data sets 
// (Opens a web browser window with the chart)
let chart' =
    [ czschool.[1975 .. 2010]; euschool.[1975 .. 2010] ]
    |> Chart.Line
    |> Chart.WithOptions (Options(legend=Legend(position="bottom")))
    |> Chart.WithLabels ["CZ"; "EU"]


let canadaStuff = series wb.Countries.Canada.Indicators.``Computer, communications and other services (% of commercial service exports)``
let chart'' =
    canadaStuff
    |> Chart.Line
    |> Chart.WithOptions (Options(legend=Legend(position="bottom")))
    |> Chart.WithLabels [wb.Countries.Canada.Indicators.``Computer, communications and other services (% of commercial service exports)``.Description]


let population = 
<<<<<<< HEAD
    (series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ] - 
     series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]) /
    series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ] * 100.0
Chart.Geo population
=======
    series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ] - 
    series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]
>>>>>>> 6562d249faa3994e0364b14baa0095e367bbfa34

let chart''' = Chart.Geo population


let [<Literal>] dataFile = __SOURCE_DIRECTORY__ + """\Data.csv"""
type People = CsvProvider<dataFile>
let people = People.Load(dataFile)
let first = people.Rows |> Seq.head
let x = first.Person_ID 
let y = first.Phone 
let z = first.Email

let [<Literal>] htmlFile = __SOURCE_DIRECTORY__ + """\html\Comparison_of_programming_languages.html"""
//let [<Literal>] htmlFile = @"https://en.wikipedia.org/wiki/Comparison_of_programming_languages"
//let [<Literal>] dependantTypes = @"https://en.wikipedia.org/wiki/Dependent_type#Comparison_of_languages_with_dependent_types"
type Languages = HtmlProvider<htmlFile>
let page = Languages.Load(htmlFile)
let data = page.Tables.``General comparison``.Rows 
            |> Array.filter(fun h -> h.``Functional`` = "Yes")
let result = 
<<<<<<< HEAD
    [for r in data ->
        r.Language, r.``Intended use``, r.Generic, r.``Object-oriented``]



[<Literal>]
let statesFile = __SOURCE_DIRECTORY__ + """\v3\States.json"""
// let statesFile = """http://greytide.azurewebsites.net/tide/v1/Models/"""
[<Literal>]
let modelsFile = __SOURCE_DIRECTORY__ + """\v3\Models.json"""
//let modelsFile = http://greytide.azurewebsites.net/tide/v1/Models/

type States = JsonProvider<statesFile>
type Models = JsonProvider<modelsFile>
let states = States.Load(statesFile)
let models = Models.Load(modelsFile)
let percentDone state  = 
    match state with
    | "Dislike" -> 0.15
    | "Assembled" -> 0.25 
    | "Prime"  -> 0.30 
    | "Varnished" -> 0.99 
    | "Paint" -> 0.90
    | "Weather" -> 0.95 
    | "Complete" -> 1.0
    | "NOS" | "Startup" | "Buy New" -> 0.0
    | x ->  0.0

let mapFactions faction =
    match faction with
    | "Tyranids" | "Gene Cult" -> 1
    | "Space Wolves" | "Deathwatch" -> 2
    | "Ork" -> 3
    | _ -> 4

let currentStates = series [for m in models ->m.Id2.ToString(), percentDone m.CurrentState]
let factions = series [for m in models -> m.Id2.ToString(), mapFactions m.Faction]
let points = series [for m in models -> m.Id2.ToString(), m.Points]
let all = Frame(["Complete";"Points";"Factions"],
                [currentStates;points;factions])

//open RProvider.utils
//R.install_packages("caret")
//R.install_packages("zoo")
open Deedle
open RDotNet
open RProvider
open RProvider.``base``
open RProvider.datasets
open RProvider.caret
open RProvider.stats

let allX = all.Columns.[["Complete";"Points"; ]] 
let fix = R.kmeans(x=allX, centers=3)
let centers = fix.AsList().["centers"]

let features =
    all
    |> Frame.filterCols (fun c _ -> c <> "Factions")
    |> Frame.mapColValues (fun c -> c.As<double>())
let targets = R.as_factor(all.Columns.["Factions"])


R.featurePlot(x = allX, y = targets, plot = "pairs")
R.featurePlot(x = centers, y = targets, plot = "pairs")


let modelVect = fix.AsList().["cluster"].AsVector()
let names = modelVect.Names
let values : int [] = modelVect.GetValue()

let keyedModels = models |> Seq.map (fun m -> m.Id2.ToString(), m.Name) |> dict
let pairs = 
    Array.zip names values
    |> Array.groupBy snd
    |> Array.map (fun (g,vs) -> vs |> Array.map (fun (k,v) -> keyedModels.[k],v))

//open System
//open System.Linq
//
//let d = models.ToDictionary(fun m -> m.Id2)
//models |> Seq.iter (fun model ->
//    printfn "Split? %s - %d" model.Name model.Points
//    match System.Console.ReadLine() |> char with
//    | 'y' -> 
//        printfn "Times?"
//        match Int32.TryParse(System.Console.ReadLine()) with
//        | true,n -> 
//            d.Remove(model.Id2) |> ignore
//            for i in 0 .. n - 1 do
//                let newM = Models.Root(
//                                        model.Id,
//                                        model.Type, 
//                                        model.Faction,
//                                        [||], 
//                                        System.Guid.NewGuid(), 
//                                        model.Name,
//                                        model.CurrentState,
//                                        model.CurrentDate, 
//                                        model.Points / n, 
//                                        [||], 
//                                        model.UserToken, 
//                                        model.Type2)
//                d.Add(newM.Id2, newM)
//        | _ -> ()
//    | _ -> ())
//"[" +  (d.Values |> Seq.map (sprintf "%A") |> String.concat ",") + "]"

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
type Person = JsonProvider<"""[{"name":"Dan", "language":"F#"},{"name":"Brian"}]""">
let samples = Person.GetSamples()
let results = samples |> Seq.head |> fun p -> p.Name, p.Language
=======
    query {
        for r in data do
        where (r.Generic = "Yes")
        select (r.Language,r.``Intended use`` )
    } |> Seq.toList

//Define a sample. 
type Person = JsonProvider<"""[{"name":"Dan", "language":"F#"}]""">
//                           ,{"name":"Dad"}
let samples = Person.GetSamples()

samples |> Array.map (fun p -> p.Name.Length + p.Language.Length) 
>>>>>>> 6562d249faa3994e0364b14baa0095e367bbfa34
