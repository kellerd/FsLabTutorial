open System
//Helpers
let addDays (date:DateTime)  num = num |> float |> date.AddDays 
let toKeyWithValue value key = key,value 
let beforeToday date = date < DateTime.Now
let byName series (name,_) = name
let byDate series (name,(date,_)) = date
let inline sumSeriesByPoints (date,total) (_,(_,newvalue:float)) = date,total+(newvalue)
