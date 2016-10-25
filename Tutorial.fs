// Before running any code, invoke Paket to get the dependencies. 
//
// You can either build the project (Ctrl + Alt + B in VS) or run 
// '.paket/paket.bootstrap.exe' and then '.paket/paket.exe install'
// (if you are on a Mac or Linux, run the 'exe' files using 'mono')
//
// Once you have packages, use Alt+Enter (in VS) or Ctrl+Enter to
// run the following in F# Interactive. You can ignore the project
// (running it doesn't do anything, it just contains this script)
#if INTERACTIVE
#load "packages/FsLab/FsLab.fsx"
#else
namespace Tutorial
module Tutorial =
#endif
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
        series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ] - 
        series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]
    
    let chart''' = Chart.Geo population

    
    let [<Literal>] dataFile = __SOURCE_DIRECTORY__ + """\Data.csv"""
    type People = CsvProvider<dataFile>
    let people = People.Load(dataFile)
    let first = people.Rows |> Seq.head
    let x = first.Person_ID 
    let y = first.Phone 
    let z = first.Email
    
    let [<Literal>] htmlFile = __SOURCE_DIRECTORY__ + """\html\Comparison_of_programming_languages.html"""
    //let htmlFile= https://en.wikipedia.org/wiki/Comparison_of_programming_languages
    type Languages = HtmlProvider<htmlFile>
    let page = Languages.Load(htmlFile)
    let data = page.Tables.``General comparison``.Rows 
                |> Array.filter(fun h -> h.``Functional`` = "Yes")
    let result = 
        query {
            for r in data do
            where (r.Generic = "Yes")
            select (r.Language,r.``Intended use`` )
        } |> Seq.toList

    //Define a sample. 
    type Person = JsonProvider<"""[{"name":"Dan", "language":"F#"}]""">
    //                           ,{"name":"Dad"}
    let samples = Person.GetSamples()

    samples |> Array.map (fun p -> p.Name.Length + p.Language.Length) |> ignore