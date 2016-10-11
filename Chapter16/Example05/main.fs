#light
open KittyAst

let parseText text = 
    let lexbuf = Lexing.from_string text
    try 
        KittyParser.start KittyLexer.token lexbuf
    with e -> 
        let pos = lexbuf.EndPos
        printf "Error near line %d, character %d\n" pos.Line pos.Column
        failwith "!"

let sample = "counter := 100; accum := 0; \n\
              while counter do \n\
              begin \n\
                  counter := counter - 1; \n\
                  accum := accum + counter \n\
              end; \n\
              print accum"

let rec evalE (env: Map<string, int>) = function
    | Val v          -> if env.ContainsKey v then env.[v]
                        else failwith ("unbound variable: " + v)
    | Int i          -> i
    | Plus  (e1, e2) -> evalE env e1 + evalE env e2
    | Minus (e1, e2) -> evalE env e1 - evalE env e2

and eval (env: Map<string, int>) = function
    | Assign (v, e) ->
         env.Add(v, evalE env e)
    | While (e, body) ->
         let rec loop env e body =
             if evalE env e <> 0 then
                 loop (eval env body) e body
             else env
         loop env e body
    | Seq stmts ->
         List.fold_left eval env stmts
    | IfThen (e, stmt) ->
         if evalE env e <> 0 then eval env stmt else env
    | IfThenElse (e, stmt1, stmt2) ->
         if evalE env e <> 0 then eval env stmt1 else eval env stmt2
    | Print e ->
         print_int (evalE env e); env

// ----------------------------

do match parseText sample with
    | Prog stmts ->
        eval Map.empty (Seq stmts) |> ignore
