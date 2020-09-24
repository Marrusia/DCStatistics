#r "../lib/FSharp.Data.DesignTime.dll"
#r "../lib/FSharp.Data.dll"
#r "../lib/System.Xml.Linq.dll"
open FSharp.Data
open System.IO

type TacviewDebriefing = XmlProvider<"../tacview_export/Tacview-20190204-202000-DCS-pisforce4.zip.xml">

let path = @"./tacview_export/"

let files = Directory.GetFiles(path, "*.xml")

let dataSet file = File.ReadAllText(file)

let missionData =
    files
    |> Array.map (fun file -> dataSet file)
    |> Array.map (fun value -> TacviewDebriefing.Parse(value))

let stats action =
    missionData
    |> Array.map (fun mission ->
        (mission.Mission.Title,
         (mission.Events
          |> Array.filter (fun event -> event.Action = action)
          |> Array.groupBy (fun event -> event.PrimaryObject.Pilot)
          |> Array.filter (fun (pilot, actions) -> Option.isSome pilot)
          |> Array.map (fun (pilot, actions) ->
              let name = pilot
              let count = actions |> Array.length
              name, count))))