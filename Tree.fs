namespace Frosty

open System
open Parser

module Tree =

    type Tree = 
        | Node of Formula * Tree List
        | Open
        | Closed

    let isLiteral formula =
        match formula with
        | Atom _ | Not(Atom _) -> true
        | _ -> false

    let sortNodes (branch: List<Formula>) = 
        List.sortBy
            (function
                | Atom _ | Not (Atom _) -> 0
                | Not (Not _) | And _ | Not (Or _) | Not (Implies _) -> 1
                | _ -> 2) branch

    let rec buildTree formulaz lits =
        let formulas = sortNodes formulaz
        match formulas with
        | Or(x, y) as z :: tail -> Node(z, [buildTree ([x] @ tail) lits; buildTree ([y] @ tail) lits])
        | And(x, y) as z :: tail -> Node(z, [buildTree ([x; y] @ tail) lits])
        | Implies(x, y) as z :: tail -> Node(z, [buildTree ([Not x] @ tail) lits; buildTree ([y] @ tail) lits])
        | Iff(x, y) as z :: tail -> Node(z, [buildTree ([x; y] @ tail) lits; buildTree ([Not x; Not y] @ tail) lits])
        | Not(Or(x, y)) as z :: tail -> Node(z, [buildTree ([Not x; Not y] @ tail) lits])
        | Not(And(x, y)) as z :: tail -> Node(z, [buildTree ([Not x] @ tail) lits; buildTree ([Not y] @ tail) lits])
        | Not(Implies(x, y)) as z :: tail -> Node(z, [buildTree ([x; Not y] @ tail) lits])
        | Not(Iff(x, y)) as z :: tail -> Node(z, [buildTree ([x; Not y] @ tail) lits; buildTree ([Not x; y] @ tail) lits])
        | Not(Not x) as z :: tail -> Node(z, [buildTree([x] @ tail) lits])
        | Atom _ | Not (Atom _) as x :: tail ->
            if Set.exists (fun y -> y = Not x || Not y = x) lits then
                Node(x, [Closed])
            else
                Node(x, [buildTree tail (Set.singleton x + lits)])
        | [] -> Open

    let createTree formulas = buildTree formulas Set.empty

    let rec checkTree tree =
        match tree with
        | Node(_, x) -> List.fold (&&) true (List.map checkTree x)
        | Closed -> true
        | Open -> false

    let bTree str = buildTree [run parseFormula str] Set.empty

    let prove str =
        buildTree
            ((fun (x: List<Formula>) -> (List.take (x.Length - 1) x) @ [Not x.[x.Length - 1]])
                <| (splitPremisesChar >> List.ofArray) str)
            Set.empty

    let rec toJson (tree: Tree) =
        match tree with
        | Node(x, y) -> @"{""formula"": """ + uglyPrint x + @""",""children"": [" + List.fold (fun x y -> if x <> "" then $"{x},{y}" else y) "" (List.map toJson y) + @"]}"
        | Open -> @"""Open"""
        | Closed -> @"""Closed"""