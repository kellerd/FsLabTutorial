#load "../packages/FsLab/FsLab.fsx"
open Deedle
open FSharp.Data

let [<Literal>] StatesFile = __SOURCE_DIRECTORY__ + """/../data/v3/States.json"""
// let [<Literal>] StatesFile = """http://greytide.azurewebsites.net/tide/v1/States"""
let [<Literal>] ModelsFile = __SOURCE_DIRECTORY__ + """/../data/v3/Models.json"""
//let [<Literal>] ModelsFile = http://greytide.azurewebsites.net/tide/v1/Models

type States = JsonProvider<StatesFile>
type Models = JsonProvider<ModelsFile>
let states = States.Load(StatesFile)
let models = Models.Load(ModelsFile)
// State to how complete it is.
let percentDone state  = 
    match state with
    | "NOS" | "Startup" | "Buy New" -> 0.0
    | "Dislike" -> 0.15
    | "Assembled" -> 0.25 
    | "Prime"  -> 0.30 
    | "Weather" -> 0.95 
    | "Paint" -> 0.90
    | "Varnished" -> 0.99 
    | "Complete" -> 1.0
    | x ->  0.0

let mapFactions faction =
    match faction with
    | "Tyranids" | "Gene Cult" -> "Tyranids"
    | "Space Wolves" | "Deathwatch" -> "Space Marines"
    | "Ork" -> "Orks"
    | _ -> "Others"
//Data prep. Magically get R to tell us some things
//Is there a relation to completion, faction and how big the model is?
let completeness = series [for m in models ->m.Id2.ToString(), percentDone m.CurrentState]
let factions = series [for m in models -> m.Id2.ToString(), mapFactions m.Faction]
let points = 
    query { 
        for m in models do
        select (m.Id2.ToString(), m.Points) 
    } |> series
//Create a frame of all the data
let data = Frame(["Complete";"Points";"Factions"],
                 [completeness;points;factions])

// Demo some RProvider
open RDotNet
open RProvider
open RProvider.graphics
open RProvider.``base``
let widgets = [ 3; 8; 12; 15; 19; 18; 18; 20; ]
let sprockets = [ 5; 4; 6; 7; 12; 9; 5; 6; ]
//TODO:
//plot widgets
//plot (widgets,sprockets)
//barplot
//hist
//pie

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//How to install packages
// open RProvider.utils
// R.install_packages("caret")
// R.install_packages("zoo")
open RProvider.caret //For featurePlot
open RProvider.stats
//Find associations of two of data's columns [["Complete";"Points"; ]] , store as xs
let xs = data.Columns.[["Complete";"Points"; ]] 
//Tags for each plot type
let factors = R.as_factor(data.Columns.["Factions"])
//TODO:
//clusters from kmeans where x=xs, centers = 3

let centers = clusters.AsList().["centers"]

//Can call it two ways, default
R.featurePlot(x = xs, y = factors, plot = "pairs")
let fpSettings = 
    Map.ofList
        [ "y",box factors; 
          "plot",box "pairs";
          "auto.key", box (R.list(Map.ofList ["columns",box 3])) ]

//Or with custom list of parameters, R is a little loose with what is available
//These are our clusters of items based on how complete they are
fpSettings 
|> Map.add "x" (box xs)
|> R.featurePlot
//These are the main centers.
//Big MC's who are 1/2 complete
//Little tyranids infantry who are mostly complete
//Medium space marine models, tanks, elites
fpSettings 
|> Map.add "x" (box centers)
|> R.featurePlot

//Show names of the groups, extract some raw data from R
let modelVect = clusters.AsList().["cluster"].AsVector()
let names = modelVect.Names
let values : int [] = modelVect.GetValue()
let keyedModels = models |> Seq.map (fun m -> m.Id2.ToString(), m.Name) |> dict

//List out groups of guys
centers
//Find the groups of guys
let pairs = 
    Array.zip names values
    |> Array.groupBy snd
    |> Array.sortBy fst
    |> Array.map (fun (g,vs) -> vs |> Array.map (fun (k,v) -> keyedModels.[k]) |> Array.countBy id |> Array.take 10)

