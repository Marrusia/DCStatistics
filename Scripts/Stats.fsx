#r "../lib/FSharp.Data.DesignTime.dll"
#r "../lib/FSharp.Data.dll"
#r "../lib/System.Xml.Linq.dll"
open FSharp.Data
open System.IO

type TacviewDebriefing = XmlProvider<"../tacview_export/Tacview-20190204-202000-DCS-pisforce4.zip.xml">

let path = @"./tacview_export/"

let files =
    Directory.GetFiles(path, "*.xml") |> Array.toSeq

let dataSet file = File.ReadAllText(file)

let data =
    files
    |> Seq.map (fun file -> dataSet file)
    |> Seq.map (fun value -> TacviewDebriefing.Parse(value))

let eventList =
    (data
     |> Seq.map (fun d -> (d.Events |> Array.toList)))

let getEventsFromData (list: list<list<'T>>) =
    let rec getEvents (list: list<list<'T>>) acc =
        match list with
        | head :: tail ->
            let acc = acc @ head
            getEvents tail acc
        | [] -> acc

    getEvents list []

let events =
    getEventsFromData (eventList |> Seq.toList)

let actions =
    events
    |> List.map (fun event -> event.Action)
    |> List.distinct

let stats action =
    events
    |> List.filter (fun event -> event.Action = action)
    |> List.groupBy (fun event -> event.PrimaryObject.Pilot)
    |> List.map (fun (pilot, actions) ->
        let name = pilot
        let count = actions |> List.length
        name, count)