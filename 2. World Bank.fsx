
#load "packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts

// Store wb as Get world bank context from WorldBankData
let wb = WorldBankData.GetDataContext()
//From wb, select a Canada from contries and pick indicators
let can = wb.Countries.Canada.Indicators
//From wb, select a mexico from contries and pick indicators
let mexico = wb.Countries.Mexico.Indicators
//Store a series of can GDP growth 
let canGDP = series can.``GDP growth (annual %)``
//Store a series of mexico GDP growth 
let mexicoGDP = series mexico.``GDP growth (annual %)``
let result = 
    //Make list of Canada's Name, canGDP 2005..2015 ; mexico's Name, mexicoGDP 2005..2015
    [
        wb.Countries.Canada.Name, canGDP.[2005..2015]
        wb.Countries.Mexico.Name, mexicoGDP.[2005..2015]
    ]
    //Make a Data Frame 
    |> frame
    //Pipe to Chart Line
    |> Chart.Line
    //Add a legend
    |> Chart.WithLegend true
// let populationChange = 
//     let ``2015`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ]
//     let ``2014`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]
//     (``2015`` - ``2014``) / ``2015`` * 100.0
//     |> abs
// //Make a Geo Chart for the population change    
// Chart.Geo populationChange

