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


let canadaStuff = series wb.Countries.Canada.Indicators.``Computer, communications and other services (% of commercial service exports)``
canadaStuff
|> Chart.Line
|> Chart.WithOptions (Options(legend=Legend(position="bottom")))
|> Chart.WithLabels [wb.Countries.Canada.Indicators.``Computer, communications and other services (% of commercial service exports)``.Description]

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

type People = CsvProvider<"Data.csv">
let people = People.GetSample()
let first = people.Rows |> Seq.head
first.Person_ID
first.Phone
first.Email

type Languages = HtmlProvider<"https://en.wikipedia.org/wiki/Comparison_of_programming_languages">
let page = Languages.Load("https://en.wikipedia.org/wiki/Comparison_of_programming_languages")
let data = page.Tables.``General comparison``.Rows 
            |> Array.filter(fun h -> h.``Functional`` = "Yes")
let result = 
    [for r in data ->
        r.Language, r.``Intended use``, r.Generic, r.``Object-oriented``]