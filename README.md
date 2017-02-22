# FsLab Tutorial


## Ionide/VsCode
This project was demoed with 
    
-  [VsCode](https://code.visualstudio.com/) Cross platform, lightweight, free
-  [Ionide](http://ionide.io/) plugin for FSharp awesomeness
- To use F#
    - Instructions on [F# Software Foundation](http://fsharp.org)
    - [Azure notebooks](https://notebooks.azure.com/) Currently free sample mode
    - tryfsharp.org

To run this project. 

- Clone or Fork repository

    git clone https://github.com/kellerd/FsLabTutorial.git

- build with [FAKE](http://fsharp.github.io/FAKE/) with shell command

    build

FAKE & Paket (Open source package manager) will download dependencies (in paket.dependencies) & Build them
    open FsLabTutorial folder in VsCode
    open file Tutorial.fsx 
    highlight pieces of code, alt-enter will send selection to FSI (F# interactive)

# Major pieces
## FAKE
Open source Make with F# Script files, a build scripting system
## Paket
Open source package manager, similar to NuGet, Cabal
## FsLab
Collection of .NET Data Science libraries for :
 - Charts
 - Data Analysis
 - Data Access
 - Statistics
##  FSharp.Data
 - Get auto-completion based on a sample document
## Deedle
 - Data frame and series library inspired by pandas
## R Provider Professional stats packages
- To install R packages:

    open RProvider.utils
    R.install_packages("caret")
    R.install_packages("zoo")

Get auto-completion on R functions & parameters

## XPlot
 - Powerful HTML5 charting
## F# Charting
 - For desktop charting

# Paket


# RProvider
Dan this is great, but it's a little rudimentary. We need some pro tools here.
Best thing about R was it was made by statisticians. The worst thing about R was that it was made by statisticians

# Additional Resources
- [MBrace](http://MBrace.io)
- [Azure notebooks](https://notebooks.azure.com/) F# run in browser without the setup
- [BigDeedle](https://github.com/BlueMountainCapital/Deedle.BigDemo) [lazy evaluated large data sources]
- [Angara](https://github.com/Microsoft/?utf8=%E2%9C%93&q=Angara)
- [The Gamma](https://thegamma.net/)- Data Jouranism, customizable data grids, auto complete data sources

- (http://FSharp.org) - The FSharp Foundation, The mission of the F# Software Foundation is to promote, protect, and advance the F# programming language, and to support and facilitate the growth of a diverse and international community of F# programmers.
- (http://functionalprogramming.slack.com) - Functional programming chat in almost any language
