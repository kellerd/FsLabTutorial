# Self Improvement with Data Science and F#
- Intro, Dan Keller
- No functional prior to 2 years ago. Took up as a way to expand my knowledge. Choose F# because it was on the same platform.
- This way, I didn't need to learn new core libraries, frameworks, package managers, it all was the same environment.
- A look at some of the open source,easy to use, community offerings to get a better handle on different sources of data.
- Demo I wanted to give
- It mainly focuses on FsLab, which itself is a collection of awesome Data Science Libraries
- How I used it to take a better look at work and make some changes

# That one app
- So I'm sure everyone has that one app.
- It's not pretty, it's not complete. You might've even re-done the app several times.
- Most apps, especially line of business, what i normally deal in are simple
  - There no real complicated domain
  - There is no business logic
  - It's just some huge side effecting page. Page after page of direct database access or update. 
- What it does have, is data. That's the money.
   - If only you could work with it a little better, massage it, there might be some hidden answer
<!--
- Any Math majors, accountants or statisticians in the crowd?
  - Ok, so you guys already know how to look at some data and get a good idea on how things are going
- Me, I'll have to poke around a bit. I'm a line of business guy. 
 - I'm all about workflows. Statistics was 10-12 years ago-->

- Now, There are some awesome things built in or with F#
  - Project Springfield, Whitebox Fuzzing for security testing
- Jet.com, trying to be an amazon competitor. Walmart bought Jet.com for it's F# pricing engine for 3 Billion
![The Louvre in Abu Dhabi](http://www.thenational.ae/storyimage/AB/20160615/ARTICLE/160619414/AR/0/AR-160619414.jpg)

# Greytide
- My website, isn't the greatest thing written in F#. Also until recently, it wasn't.
- http://greytide.azurewebsites.net
- Basically a time tracker. 

- The goal wasn't creating an awesome program for everyone, or just tracking my progress I could've used almost anything, it was for learning.
 - Use it for testing different frameworks, tools. Things i'm not allowed to use at work, but may in the future.
- Trying to do the practical thing, expand knowledge, make opinions on technology it's what we're paid for.
- I was using stuff like 
- D3 drawing library, Angular, C# Web Api, Copying some code from a SPA course
- Browser storage, local caching, Then NoSQL, Cloud providers
- It was 2 years ago, in client javascript land, 2 years might as well be 2 eons ago
- Now, back to my site. It's pretty bad on the surface. 
 - Most of what I was trying to learn was in the backend.
- How to make it better
  - I still have great thoughts into basically making it into a Kanban board of some sort, but things other than css & javascript things have peeked my interests
  - My first part of learning comes from the F# Organization's Mentorship program. I was paired with one of the authors of Expert F#, now it wasn't F#'s inventor, but if he's paired with him, he must be good right?
  - Was a six week program, some of the mentees have different projects, some work on the F# Compiler, some are just learning the basics.
  - I aimed somewhere in the middle and learned a lightweight F# combinator webserver to replace all that C#. Even a straight port of C#'s logic removed -140k lines of packages, ect)



# VsCode/Ionide
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
- Bar chart, but shorter
- Moving mean
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
# Summary
- Use, learn some statistics I learned in the past
- Without having to remember javascript, css, some random drawing framework, that you need some other framework to use easily.
- Learned what I like painting, where are my bottlenecks. 
  - Change the way I paint.  Assembly line the easy stuff to a certain point, spend time on the details.
  - And from the data I could find out what I could easily offer as a paid painting service for people. 
# Alternate Resources
- Mention MBrace.io
- Mention Azure Notebooks
- Mention BigDeedle
- Mention Angara https://github.com/Microsoft/?utf8=%E2%9C%93&q=Angara

- FSharp.org
- functionalprogramming.slack.com