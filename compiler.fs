module Compiler

// Find the position of variable x on the runtime stack:
let rec varpos x = function
    | []       -> failwith ("unbound: " + x)
    | y :: env -> if x = y then 0 else 1 + varpos x env

// Generating fresh labels:
let mutable labelCounter = 0
let newLabel() =
    let this = labelCounter
    labelCounter <- this + 1
    "L" + string(this)

// Remove n function-call arguments after a call.
// After the call returns, the result is on top and old arguments are below it.
let rec popArgs n =
    if n = 0 then
        []
    else
        [Asm.ISWAP] @
        [Asm.IPOP] @
        popArgs (n - 1)

// Compile an expression:
let rec comp env = function
    | Syntax.INT i ->
        [Asm.IPUSH i]

    | Syntax.VAR x ->
        [Asm.ILOAD (varpos x env)]

    | Syntax.BOOL b ->
        [Asm.IPUSH (if b then 1 else 0)]

    | Syntax.ADD (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.IADD]

    | Syntax.SUB (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.ISUB]

    | Syntax.MUL (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.IMUL]

    | Syntax.DIV (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.IDIV]

    | Syntax.MOD (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.IMOD]

    | Syntax.NEG e ->
        [Asm.IPUSH 0] @
        comp ("" :: env) e @
        [Asm.ISUB]

    | Syntax.EQ (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.IEQ]

    | Syntax.NEQ (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.IEQ] @
        [Asm.IPUSH 0] @
        [Asm.IEQ]

    | Syntax.LT (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.ILT]

    | Syntax.LTE (e1, e2) ->
        comp env e2 @
        comp ("" :: env) e1 @
        [Asm.ILT] @
        [Asm.IPUSH 0] @
        [Asm.IEQ]

    | Syntax.GT (e1, e2) ->
        comp env e2 @
        comp ("" :: env) e1 @
        [Asm.ILT]

    | Syntax.GTE (e1, e2) ->
        comp env e1 @
        comp ("" :: env) e2 @
        [Asm.ILT] @
        [Asm.IPUSH 0] @
        [Asm.IEQ]

    | Syntax.LET (x, e1, e2) ->
        comp env e1 @
        comp (x :: env) e2 @
        [Asm.ISWAP] @
        [Asm.IPOP]

    | Syntax.IF (e1, e2, e3) ->
        let thenLabel = newLabel()
        let endLabel  = newLabel()
        comp env e1 @
        [Asm.IJMPIF thenLabel] @
        comp env e3 @
        [Asm.IJMP endLabel] @
        [Asm.ILAB thenLabel] @
        comp env e2 @
        [Asm.ILAB endLabel]

    | Syntax.AND (e1, e2) ->
        let evalE2 = newLabel()
        let endLabel = newLabel()
        comp env e1 @
        [Asm.IJMPIF evalE2] @
        [Asm.IPUSH 0] @
        [Asm.IJMP endLabel] @
        [Asm.ILAB evalE2] @
        comp env e2 @
        [Asm.ILAB endLabel]

    | Syntax.OR (e1, e2) ->
        let trueLabel = newLabel()
        let endLabel = newLabel()
        comp env e1 @
        [Asm.IJMPIF trueLabel] @
        comp env e2 @
        [Asm.IJMP endLabel] @
        [Asm.ILAB trueLabel] @
        [Asm.IPUSH 1] @
        [Asm.ILAB endLabel]

    | Syntax.READ ->
        [Asm.IREAD]

    | Syntax.WRITE e ->
        comp env e @
        [Asm.ILOAD 0] @
        [Asm.IWRITE]

    | Syntax.CALL (f, es) ->
        compArgs env es @
        [Asm.ICALL f] @
        popArgs (List.length es)

// Compile function-call arguments from left to right.
// Each compiled argument leaves one extra value on the stack,
// so later arguments must be compiled in an extended environment.
and compArgs env = function
    | [] ->
        []

    | e :: es ->
        comp env e @
        compArgs ("" :: env) es

// Compile a whole program.
let rec compProg = function
    | ([], e1) ->
        comp [] e1 @
        [Asm.IHALT]

    | ((f, (xs, e)) :: funcs, e1) ->
        compProg (funcs, e1) @
        [Asm.ILAB f] @
        comp ("" :: List.rev xs) e @
        [Asm.ISWAP] @
        [Asm.IRETN]