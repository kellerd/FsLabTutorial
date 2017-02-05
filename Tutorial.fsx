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
abs (czschool - euschool)
|> Series.sort
|> Series.rev
|> Series.take 5

// Plot a line chart comparing the two data sets 
// (Opens a web browser window with the chart)
[ czschool.[1975 .. 2010]; euschool.[1975 .. 2010] ]
|> Chart.Line
|> Chart.WithOptions (Options(legend=Legend(position="bottom")))
|> Chart.WithLabels ["CZ"; "EU"]


let canadaStuff = series wb.Countries.Canada.Indicators.``Computer, communications and other services (% of commercial service exports)``
canadaStuff
|> Chart.Line
|> Chart.WithOptions (Options(legend=Legend(position="bottom")))
|> Chart.WithLabels [wb.Countries.Canada.Indicators.``Computer, communications and other services (% of commercial service exports)``.Description]


let population = 
    series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ] - 
    series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]
Chart.Geo population

[<Literal>]
let dataFile = __SOURCE_DIRECTORY__ + """\Data.csv"""
type People = CsvProvider<dataFile>
let people = People.Load(dataFile)
let first = people.Rows |> Seq.head
first.Person_ID
first.Phone
first.Email

[<Literal>]
let htmlFile = __SOURCE_DIRECTORY__ + """\html\Comparison_of_programming_languages.html"""
//let htmlFile= https://en.wikipedia.org/wiki/Comparison_of_programming_languages
type Languages = HtmlProvider<htmlFile>
let page = Languages.Load(htmlFile)
let data = page.Tables.``General comparison``.Rows 
            |> Array.filter(fun h -> h.``Functional`` = "Yes")
let result = 
    [for r in data ->
        r.Language, r.``Intended use``, r.Generic, r.``Object-oriented``]



[<Literal>]
let statesFile = __SOURCE_DIRECTORY__ + """\v1\States.json"""
// let statesFile = """http://greytide.azurewebsites.net/tide/v1/Models/"""
[<Literal>]
let modelsFile = __SOURCE_DIRECTORY__ + """\v1\Models.json"""
//let modelsFile = http://greytide.azurewebsites.net/tide/v1/Models/

type States = JsonProvider<statesFile>
type Models = JsonProvider<modelsFile>
let states = States.Load(statesFile)
let models = Models.Load(modelsFile)

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