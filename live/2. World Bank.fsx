
#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts

//TODO:
// Get world bank context from WorldBankData, store in wb
//From wb, select a Canada from contries and pick indicators
//From wb, select a mexico from contries and pick indicators
//Store a series of can GDP growth 
//Store a series of mexico GDP growth 

let result = 
    //Make list of Canada's Name, canGDP 2005..2015 ; mexico's Name, mexicoGDP 2005..2015
    [
        //wb.Countries.Canada.Name, canGDP.[2005..2015]
        //wb.Countries.Mexico.Name, mexGDP.[2005..2015]
    ]
    //Make a Data Frame 
    //Pipe to Chart Line
    //Add a legend

// let populationChange = 
//     let ``2015`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2015] ]
//     let ``2014`` = series [for c in wb.Countries -> c.Name, c.Indicators.``Population, total``.[2014] ]
//     (``2015`` - ``2014``) / ``2015`` * 100.0
//     |> abs
// //Make a Geo Chart for the population change    
// Chart.Geo populationChange

