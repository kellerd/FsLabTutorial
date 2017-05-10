#I "../packages/FSharp.Charting"
#load "FSharp.Charting.fsx"
#load "EventEx.fsx"
//#r "../packages/FSharp.Control.AsyncSeq/lib/net45/FSharp.Control.AsyncSeq.dll"

open System
open System.Drawing
open FSharp.Charting
open FSharp.Control


let data = [ for x in 0 .. 99 -> (x,float (x*x)) ]
let data2 = [ for x in 0 .. 99 -> (x,sin(float x / 10.0)) ]
let data3 = [ for x in 0 .. 99 -> (x,cos(float x / 10.0)) ]

LiveChart.Line (Event.cycle 1000 [data; data2; data3])


let rnd = new System.Random()
let rand() = rnd.NextDouble()
let data4 = Event.clock 10 |> Event.map (fun x -> x, rand())

LiveChart
  .FastLineIncremental(data4,Name="Moving")
  .WithXAxis(Enabled=false)
  .WithYAxis(Enabled=false)

let incData = Event.clock 10 |> Event.map (fun x -> (x, x.Millisecond))

LiveChart
  .FastLineIncremental(incData,Name="Moving")
  .WithXAxis(Enabled=false)
  .WithYAxis(Enabled=false)