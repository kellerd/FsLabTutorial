# Self Improvement with Data Science and F#
- Hi Everyone, my name is Dan Keller
- I work at Transport Canada. I know goverment, ick.  Ok, ok, its not all that bad.
- This is going to be a look at some of the open source,easy to use, community offerings in F# to get a better handle on different sources of data
- It's the demo I wanted to give several months ago
- It mainly focuses on FsLab, which itself is a collection of awesome Data Science Libraries
- And How I used it to take a better look at work I was doing and expose some stories from the data
### Why F#
- Ok, I know, I know why F# 
 - I started in FP about 2 years ago. 
 - This kind of stuff was just hidden away from me.
 - I didn't even know it existed.
 - There is something about google, where it will only give you topics you've already seen. If you search for politics, it will only give you information based on views it's already been able to determine from your search practices
 - I somehow stumbled on it and took up as a way to expand my knowledge.
  And was able to choose F# because it was on the same platform as we were already using
- This way, I didn't need to learn new core libraries, frameworks, package managers, it all was the same environment. 
- If I was in a Java shop, chances are I'd be giving a Scala talk right about now.
- Right tool for the right job.

# Most of my apps
- Now, I said I work in government. The boring side, corporate services.
- Most apps, especially line of business, what i normally deal in are simple
  - There no real complicated domain
  - There is no business logic
  - It's just some huge side effecting page after page of direct database access or update. 

#That one app
- But there is this one app. I'm sure everyone has that one app.
- It's not pretty, it's not complete. You might've even re-done the app several times.
- Before I show you my one app, I must give you a warning
# Disclaimer
- There are some awesome things do exist built with F#
  - Project Springfield, Whitebox Fuzzing for security testing
- Jet.com, trying to be an amazon competitor. Walmart bought Jet.com for it's F# pricing engine last year
![The Louvre in Abu Dhabi](http://www.thenational.ae/storyimage/AB/20160615/ARTICLE/160619414/AR/0/AR-160619414.jpg)

# Greytide
- My website, isn't the greatest thing written in F#. Also until last october, it wasn't.  I'll thank the F# foundation for that motivation.
- Yea, it's bad
- How to make it better
  - I still have great thoughts into basically making it into a Kanban board of some sort, but things other than css & javascript things have peeked my interests
  <!--- My first part of learning comes from the F# Organization's Mentorship program. I was paired with one of the authors of Expert F#, now it wasn't F#'s inventor, but if he's paired with him, he must be good right?
  - Was a six week program, some of the mentees have different projects, some work on the F# Compiler, some are just learning the basics.
  - I aimed somewhere in the middle and learned a lightweight F# combinator webserver to replace all that C#. Even a straight port of C#'s logic removed -140k lines of packages, ect)-->

- http://greytide.azurewebsites.net

#I know pretty bad
- The goal wasn't creating an awesome program for everyone, or just tracking my progress I could've used almost anything, it was for learning.
#Stretch goals
- I know where I'd want to take it.
- Basically Trello with some personal features
# Goals
 - What I really used for was testing different frameworks, tools. Things i'm not allowed to use at work, but may in the future.
- Trying to do the practical thing, expand knowledge, make opinions on technology it's what we're paid for.
- I was using stuff like 
- D3 drawing library, Angular, C# Web Api, Copying some code from a SPA course
- Browser storage, local caching, Then NoSQL, Cloud providers
- But that was 2 years ago, in javascript land, 2 years might as well be 2 eons ago
- Now, back to my site. It's pretty bad on the surface. 
 - Most of what I was trying to learn was in the backend.
#Value for me
- What it does have, is data. That's the money.
   - If only you could work with it a little better, massage it, there might be some hidden answer


# VsCode/Ionide
- Before we get into the real 
- [VsCode](https://code.visualstudio.com/) Cross platform, lightweight, free
- [Ionide](http://ionide.io/) plugin for FSharp awesomeness
- Paket, F# package manager. Nuget only goes so far, paket does nuget


# Demo
- [FsLab](http://fslab.org)

- World bank providers
  - Pick two countries
  - \`` Double backtick quotation ``
  - Math like abs, - 
- Series, index by years, sort, take, ect
- Chart Line, Labels
- Chart Geo

- Html Provider
 - Dual with wiki page. (Web scraping)
- Various ways to query? maybe
- Csv Provider
 - Changing column name
- There are others (Management/Registry?)
- JsonProvider light
 - Provide single sample
  - make Array
  - Change field to optional

- I wanted something so I could track my progress with my little hobby. It's warhammer. Any tabletop wargaming fans out there?
 - Warhammer? Warmachine? Even D&D. Or on the hobby side, trains, model airplanes
 - For those of you not familiar. 
 - This game, my friends got my into it in school. Set up on your kitchen table, have battles, strategy, tactics my army vs yours, roll buckets of dice. Beer & Pizza All great fun.
 

- Now they also come in different sizes, fantasy, sci-fi, aliens, tanks, super humans, not so super humans. Good variety. The better they are, the more points they are worth.
- So one part of this talk is about Self improvement and learning.
 - Here is one of the keys. Pick a topic, pick something you know really well. Hobby related.
 - Use that as your topic while learning. I picked my hobby. The original author was scheduling conferences, so thats the topic he picked.
 - That wasn't really my schitck, and it made me focus a little better, it also lead me to other things related that I wanted to learn.
 - I did make one good decision, store history of events

- GreyTide service
 - Show what some properties mean
- Infinite Seq
- Moving mean
- Bar chart, but shorter
- Fill with missing
- Get certain columns
- Trendlines (primed, completed, assembled high. Painted not so much)
 - Change version
- Try online v2, Whoops. v3 without auth


# RProvider
- Dan this is great, but it's a little rudimentary. We need some pro tools here.
- Who knows R? I don't either
- Best thing about R was it was made by statisticians. The worst thing about R was that it was made by statisticians
- Install
- R Histogram, pie
- Basic statistics
- Kmeans
 - Centers (Heroes, Big dudes, Infantry)
 - Get some data out of R, some models 
# Lessons
- Spike in work from Airbrush
- Way industry is going. Marketing is working?
- Whats missing - Time tracking.
- Start painting models for people, it's a current business for other people.

# Summary
- Use, learn some statistics I learned in the past
- Without having to remember javascript, css, some random drawing framework, that you need some other framework to use easily.
- Learned what I like painting, where are my bottlenecks. 
  - Change the way I paint.  Assembly line the easy stuff to a certain point, spend time on the details.
  - And from the data I could find out what I could easily offer as a paid painting service for people. 

# Alternate Resources
- MBrace.io
- Azure Notebooks
- BigDeedle
- Angara https://github.com/Microsoft/?utf8=%E2%9C%93&q=Angara

//http://fsprojects.github.io/FSharp.Data.TypeProviders/sqldata.html
//http://bluemountaincapital.github.io/FSharpRProvider/mac-and-linux.html
//http://fsprojects.github.io/SQLProvider/
//http://fsprojects.github.io/FSharp.Data.SqlClient/
//http://fsprojects.github.io/DynamicsCRMProvider/
//Exploring StackOverflow http://evelinag.com/ExploringStackOverflow/#/ - https://youtu.be/qlKZKN7il7c


// Most weekend

//     Minecraft : 1.19
//     LWJGL : 1.12
//     SFML (Simple and Fast Multimedia Library) : 1.06
//     D : 1.04
//     pygame : 1.03
// Most weekday

//     SQL Server Reporting Services 2008, r2 : 0.11
//     Infragistics : 0.13
//     SQL Server Reporting Services 2008 : 0.13
//     Axapta : 0.13
//     DocusignAPI : 0.14
// Functional language ratio (> 1 is weekend, <1 is weekday)
//http://evelinag.com/exploring-stackoverflow/#/0/41



//Star wars http://evelinag.com/blog/2015/12-15-star-wars-social-network/#.WA-N4nqTTOU - https://youtu.be/EI8a6hFFRGQ



- FSharp.org
- functionalprogramming.slack.com
