
#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts

// Store wb as Get world bank context from WorldBankData
let wb = WorldBankData.GetDataContext()
let can = wb.Countries.Canada.Indicators
let mexico = wb.Countries.Mexico.Indicators
let canGDP = series can.``GDP growth (annual %)``
let mexicoGDP = series mexico.``GDP growth (annual %)``
let result = 
    [
        wb.Countries.Canada.Name, canGDP.[2005..2015]
        wb.Countries.Mexico.Name, mexicoGDP.[2005..2015]
    ]
    |> frame
    |> Chart.Line
    |> Chart.WithLegend true
// let populationChange = 
//     let ``2015`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ]
//     let ``2014`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]
//     (``2015`` - ``2014``) / ``2015`` * 100.0
//     |> abs
// Chart.Geo populationChange

