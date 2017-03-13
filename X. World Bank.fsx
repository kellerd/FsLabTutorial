
#load "packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts

// Store wb as Get world bank context from WorldBankData
let wb = WorldBankData.GetDataContext()
let uk = wb.Countries.``United Kingdom``.Indicators
From wb, select a Canada from contries and pick indicators
let can = wb.Countries.Canada.Indicators
//From wb, select a UK from contries and pick indicators
let uk = wb.Countries.``United Kingdom``.Indicators
//Store a series of can GDP growth 
let canGDP = series can.``GDP growth (annual %)``
//Store a series of UK GDP growth 
let ukGDP = series uk.``GDP growth (annual %)``
let result = 
    //Make list of Canada's Name, canGDP 2005..2015 ; UK's Name, ukGDP 2005..2015
    [
        wb.Countries.``Canada``.Name, canGDP.[2005..2015]
        wb.Countries.``United Kingdom``.Name, ukGDP.[2005..2015]
    ]
    //Make a Data Frame 
    |> frame
    //Pipe to Chart Line
    |> Chart.Line
    //Add a legend
    |> Chart.WithLegend true
let populationChange = 
    let ``2015`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ]
    let ``2014`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]

    (``2015`` - ``2014``) / ``2015`` * 100.0
    |> abs
//Make a Geo Chart for the population change    
Chart.Geo populationChange

