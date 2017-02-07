#load "packages/FsLab/FsLab.fsx"


open Deedle
open FSharp.Data

// Get countries in the Euro area
let wb = WorldBankData.GetDataContext()
let countries = wb.Regions.``Euro area``

// Get a frame with debts as a percentage of GDP 
let debts = 
  [ for c in countries.Countries ->
      let debts = c.Indicators.``Central government debt, total (% of GDP)``
      c.Name => series debts ] |> frame
(**
The above snippet defines a `debt` value, which is a data frame with years as 
the row index and country names as the column index. You can use the 
`include-value` command to include a table summarizing the frame data:
*)

(*** include-value:debts ***)

(**
As you can see, you can even include simple F# expressions in the command. By default,
FsLab Journal uses the "G4" format string, but if you want to use other format string,
you can specify it in the `Main.fs` file when calling `Journal.Process`. You can also 
embed LaTeX in your reports and write
(for more options [see the documentation](http://tpetricek.github.io/FSharp.Formatting/sideextensions.html)):

$$$
R = \frac{\mathit{round}(100 \times \mathit{debt})}{100}


Sample data analysis with Deedle
--------------------------------

You can also use `define-output` to give a name to a code block. When the code 
block is an expression that returns a value, you can use `include-it` to 
include the formatted result:

*)
(*** define-output:top4 ***)
let recent = debts.Rows.[2005 ..]

recent
|> Stats.mean
|> Series.sort
|> Series.rev
|> Series.take 4
|> round
(*** include-it:top4 ***)

(**
Here, we calculate means of debts over years starting with 2005, take the 4
countries with the greatest average debt and round the debts.

Calculating with Math.NET
-------------------------

If you want to implement a more complex calculation, you can turn a Deedle frame
or series to a Math.NET matrix or vector, respectively, and use the linear algebra
features of Math.NET. For example:
*)
open MathNet.Numerics.LinearAlgebra

// Create matrix from debts & vector from means
let debtsMat = debts |> Frame.fillMissingWith 0.0 |> Frame.toMatrix
let avgVect = debts |> Stats.mean |> Series.toVector

// Multiply debts per year by means
debtsMat * avgVect

(*** include-value:debtsMat ***)
(**
FsLab Journal also supports embedding of matrices and vectors. Here, you can see
how the matrix with debts (filled with zeros for missing values) is formatted as a
matrix. Then, the code shows how to use a simple matrix multiplication using 
Math.NET.

Embedding sample F# Charting charts
-----------------------------------

The generated report can also automatically embed charts created using the 
F# Charting library. Here, we plot the debts of the 3 countries with the largest
debt based on the previous table:

*)
(*** define-output:chart ***)
open FSharp.Charting

// Combine three line charts and add a legend
Chart.Combine(
  [ Chart.Line(recent?Cyprus, Name="Cyprus")
    Chart.Line(recent?Malta, Name="Malta")
    Chart.Line(recent?Greece, Name="Greece") ])
  .WithLegend()
(*** include-it:chart ***)

(**
Interoperating with R 
---------------------

If you have R installed, you can use the [R type provider][rprovider] in FsLab
experiments. The FsLab runner captures drawing done on R graphical device 
automatically and so you can also embed charts using the `include-output` command.

The R type provider looks at packages available in your R installation and makes
them available as namespaces under `RProvider`. The following example imports a 
couple of packages and then draws a histogram from all debts in the entire data set:
*)

(*** define-output:hist ***)
open RProvider
open RProvider.graphics
open RProvider.stats
open RProvider.``base``

let widgets = [ 3; 8; 12; 15; 19; 18; 18; 20; ]
let sprockets = [ 5; 4; 6; 7; 12; 9; 5; 6; ]

R.plot(widgets)

R.plot(widgets, sprockets)

R.barplot(widgets)

R.hist(sprockets)

R.pie(widgets)

R.hist(debts.GetAllValues<float>())
(*** include-output:hist ***)

(**
R can be useful for drawing charts, but also for accessing a wide range of statistical
functions that are not available in other F# libraries. For example, we can calculate 
correlation between the different countries based on debts. We do this only for recent
years and we first drop columns that contain missing values:
*)
let rdf = R.as_data_frame(R.cor(recent.DropSparseColumns()))
let cors = rdf.GetValue<Frame<string, string>>()
(**
The `cor` function returns a matrix and we first convert it (in R) to data frame using
`as_data_frame`. Then we convert the R data frame into Deedle frame using the 
`GetValue` function, which takes a type parameter specifying the required type on the
F# side.

Now, we can use `stack` function to get a frame containing row keys, column keys and 
values. This gives us a list of pairs of countries and their correlation. We get only
pairs where the first country is before the second (alphabetically) to remove self-correlations
and duplicates. Then we sort the countries and take the 3 most correlated:
*)

(*** define-output:cor ***)
cors
|> Frame.stack
|> Frame.filterRowValues (fun row -> 
    row.GetAs<string>("Row") < row.GetAs<string>("Column") )
|> Frame.sortRowsBy "Value" ((*) -1.0)
|> Frame.take 3

(*** include-it:cor ***)

type Recall = CsvProvider<"""C:\Users\diese\Downloads\vrdb_full_monthly.csv""", InferRows=2900>
let data = Recall.GetSample().Rows

let x = series [for row in data -> row.RECALL_NUMBER_NUM => row.MAKE_NAME_NM] 
let y = series [for row in data -> row.RECALL_NUMBER_NUM => row.MODEL_NAME_NM]
let z = series [for row in data -> row.RECALL_NUMBER_NUM => row.SYSTEM_TYPE_ETXT ]

let frame = frame ["Make",x
                   "Model",y
                   "System",z]
(**
More about the FsLab journal runner
-----------------------------------

When you hit **F5** in Visual Studio, the FsLab runner automatically processes all 
`*.fsx` and `*.md` files in the root directory of your project. The generated files 
are placed in the `output` folder (together with all the styles and JavaScript files 
that it requires). Then, the runner opens your default web browser with the generated
file.

If you have multiple files, the runner automatically generates an index file with
links to all your notebooks and opens this instead. You can also create your 
own index file by adding a file named `Index.fsx` or `Index.md` (if you only 
want to write Markdown text in your index).

### Command line

The runner can be also invoked from command line - the template includes a simple
[FAKE][fake] build script that is copied to the root directory of your project
(if you modify this, it will be overwritten). The build script supports the following
commands:

 - `build html` Generate HTML output for all scripts 
   and store the results in `output` folder

 - `build latex` Generate LaTeX output for all scripts 
   and store the results in `output` folder

 - `build pdf` Generate LaTeX output as when using `build latex` and then run `pdflatex` 
   on the files (this only works when you have `pdflatex` accessible in `PATH`

 [fslab]: http://www.nuget.org/packages/FsLab
 [fsfmt]: http://tpetricek.github.io/FSharp.Formatting/
 [rprovider]: http://bluemountaincapital.github.io/FSharpRProvider/
 [deedle]: http://bluemountaincapital.github.io/Deedle/
 [fschart]: http://fsharp.github.io/FSharp.Charting/
 [fsdata]: http://fsharp.github.io/FSharp.Data/
 [mathnet]: http://numerics.mathdotnet.com/
 [fake]: http://fsharp.github.io/FAKE/

*)