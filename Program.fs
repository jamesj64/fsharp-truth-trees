namespace Frosty

open System
open System.IO
open Parser
open Tree

module Program =

    [<EntryPoint>]
    let main argv =
        printfn "%s" <| (prove >> toJson) "P \n P ⇒ Q \n Q"
        0 // return an integer exit code