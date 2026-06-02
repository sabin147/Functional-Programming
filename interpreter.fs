// Part B.1: Dynamic type checker / interpreter
// Supports arithmetic, booleans, comparisons, let, if, and, or.

module Interpreter

// Runtime values in the dynamically typed language
type value =
    | IntVal of int
    | BoolVal of bool

// Abstract syntax tree for expressions
type exp =
    | INT of int
    | BOOL of bool
    | VAR of string
    | ADD of exp * exp
    | SUB of exp * exp
    | MUL of exp * exp
    | DIV of exp * exp
    | MOD of exp * exp
    | NEG of exp
    | EQ of exp * exp
    | NEQ of exp * exp
    | LT of exp * exp
    | LTE of exp * exp
    | GT of exp * exp
    | GTE of exp * exp
    | AND of exp * exp
    | OR of exp * exp
    | LET of string * exp * exp
    | IF of exp * exp * exp

type env = (string * value) list

// Look up a variable in the environment
let lookup x env =
    match List.tryFind (fun (y, _) -> x = y) env with
    | Some (_, v) -> v
    | None -> failwith ("unbound variable: " + x)

// Runtime type checks
let expectInt v =
    match v with
    | IntVal i -> i
    | BoolVal _ -> failwith "type error: expected int but got bool"

let expectBool v =
    match v with
    | BoolVal b -> b
    | IntVal _ -> failwith "type error: expected bool but got int"

let valueToString v =
    match v with
    | IntVal i -> string i
    | BoolVal b -> string b

// Evaluate an expression and perform type checks at runtime
let rec eval env e =
    match e with
    | INT i ->
        IntVal i

    | BOOL b ->
        BoolVal b

    | VAR x ->
        lookup x env

    | ADD (e1, e2) ->
        IntVal (expectInt (eval env e1) + expectInt (eval env e2))

    | SUB (e1, e2) ->
        IntVal (expectInt (eval env e1) - expectInt (eval env e2))

    | MUL (e1, e2) ->
        IntVal (expectInt (eval env e1) * expectInt (eval env e2))

    | DIV (e1, e2) ->
        IntVal (expectInt (eval env e1) / expectInt (eval env e2))

    | MOD (e1, e2) ->
        IntVal (expectInt (eval env e1) % expectInt (eval env e2))

    | NEG e ->
        IntVal (- expectInt (eval env e))

    | LT (e1, e2) ->
        BoolVal (expectInt (eval env e1) < expectInt (eval env e2))

    | LTE (e1, e2) ->
        BoolVal (expectInt (eval env e1) <= expectInt (eval env e2))

    | GT (e1, e2) ->
        BoolVal (expectInt (eval env e1) > expectInt (eval env e2))

    | GTE (e1, e2) ->
        BoolVal (expectInt (eval env e1) >= expectInt (eval env e2))

    | EQ (e1, e2) ->
        let v1 = eval env e1
        let v2 = eval env e2
        match v1, v2 with
        | IntVal i1, IntVal i2 -> BoolVal (i1 = i2)
        | BoolVal b1, BoolVal b2 -> BoolVal (b1 = b2)
        | _ -> failwith "type error: == requires two ints or two bools"

    | NEQ (e1, e2) ->
        let v1 = eval env e1
        let v2 = eval env e2
        match v1, v2 with
        | IntVal i1, IntVal i2 -> BoolVal (i1 <> i2)
        | BoolVal b1, BoolVal b2 -> BoolVal (b1 <> b2)
        | _ -> failwith "type error: != requires two ints or two bools"

    | AND (e1, e2) ->
        let b1 = expectBool (eval env e1)
        if b1 then
            BoolVal (expectBool (eval env e2))
        else
            BoolVal false

    | OR (e1, e2) ->
        let b1 = expectBool (eval env e1)
        if b1 then
            BoolVal true
        else
            BoolVal (expectBool (eval env e2))

    | LET (x, e1, e2) ->
        let v1 = eval env e1
        eval ((x, v1) :: env) e2

    | IF (e1, e2, e3) ->
        let b = expectBool (eval env e1)
        if b then
            eval env e2
        else
            eval env e3

let run e =
    eval [] e