```markdown
# Functional Language Compiler & Virtual Machine

A complete compiler and virtual machine implementation for a custom functional programming language, developed for the **Functional Programming and Language Implementations (FPLI)** course at Roskilde University.

**Core Achievement:** Bridges high-level functional source code with low-level stack-machine execution through a complete compilation pipeline – from parsing to bytecode generation to execution.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Quick Start](#quick-start)
- [Language Specification](#language-specification)
- [Compilation Example](#compilation-example)
- [Key Technical Implementations](#key-technical-implementations)
- [Calling Convention](#calling-convention)
- [Short-Circuit Evaluation](#short-circuit-evaluation)
- [Design Decisions](#design-decisions)
- [Troubleshooting](#troubleshooting)
- [Building & Testing](#building--testing)
- [Academic Context](#academic-context)
- [Quick Reference](#quick-reference)
- [Glossary](#glossary)
- [References](#references)

---

## Overview

This project implements a **custom functional programming language** through three complementary execution strategies:

| Component | Role | Implementation |
|-----------|------|----------------|
| **Compiler** | AST → Stack bytecode | Recursive AST traversal with environment tracking |
| **Virtual Machine** | Bytecode execution | Stack-based architecture with 20+ instructions |
| **Interpreter** | Direct evaluation | Dynamically-typed runtime with pattern matching |

**The language supports:**
- Arithmetic expressions (`+`, `-`, `*`, `/`, `%`)
- Boolean logic (`&&`, `||`, `==`, `!=`, `<`, `<=`, `>`, `>=`)
- Control flow (`if-then-else`)
- First-order functions (multi-argument, with calling conventions)
- I/O operations (`read`, `write`)

---

## Architecture

```
Source Code (string)
       ↓
  [FsLex Lexer]
       ↓
  Token stream
       ↓
  [FsYacc Parser]
       ↓
  AST (Expr Discriminated Union)
       ↓
  [Compiler.fs - Pattern Matching + Environment]
       ↓
  Bytecode (int array)
       ↓
  [VM.dll Stack Machine]
       ↓
  Result Value

  (Interpreter.fs provides an alternative direct evaluation path)
```

**Design Philosophy:** Each phase is a pure function – input transformed to output without mutation. No global state, no side effects except explicit I/O.

---

## Features

### Language Features

| Category | Features |
|----------|----------|
| **Types** | `int`, `bool`, `unit` with dynamic type checking |
| **Arithmetic** | `+`, `-`, `*`, `/`, `%`, unary negation |
| **Comparisons** | `==`, `!=`, `<`, `<=`, `>`, `>=` |
| **Logic** | `&&`, `||` (short-circuited) |
| **Control Flow** | `if-then-else` expressions |
| **Functions** | First-order, multi-argument (0 to N parameters) |
| **I/O** | `read` (console input), `write` (console output) |

### Compiler Features

- ✅ AST to bytecode mapping for all language constructs
- ✅ Environment management (symbol → stack offset tracking)
- ✅ Short-circuit optimization using jump labels (`ILAB`, `IJMPIF`)
- ✅ Standardized calling convention for function calls

### Virtual Machine Features

- ✅ Stack architecture for expression evaluation
- ✅ Unconditional and conditional jumps
- ✅ Function calls with return address management
- ✅ Runtime type tags for dynamic checking

---

## Technology Stack

| Component | Technology |
|-----------|------------|
| Language | F# 5.0 / .NET 6.0+ |
| Lexer Generator | FsLex |
| Parser Generator | FsYacc |
| Virtual Machine | `vm.dll` (.NET assembly) |
| Assembler | `asm.dll` (custom instruction set) |
| Development Environment | VS Code + Ionide / dotnet fsi |

---

## Project Structure

```
Functional-Programming/
│
├── Syntax.fs              # AST definition (discriminated unions)
├── Compiler.fs            # Core compilation logic
├── Interpreter.fs         # Direct evaluator (Part B)
├── main.fsx               # Entry point & test harness
│
├── Asm.dll                # Instruction set & assembler
├── VM.dll                 # Stack machine executor
│
├── lexer.fsl              # FsLex token definition
├── parser.fsy             # FsYacc grammar specification
│
├── mp.pdf                 # Mandatory assignment description
├── mp.zip                 # Project template
│
└── docs/
    └── Functional_Programming_Report.pdf
```

### Core File Responsibilities

| File | Primary Responsibility | Key Types/Functions |
|------|----------------------|---------------------|
| `Syntax.fs` | AST definition | `Expr`, `program` discriminated unions |
| `Compiler.fs` | Code generation | `compExpr`, `compProg`, environment as `int list` |
| `Interpreter.fs` | Direct execution | `eval`, `Value` type with type tags |
| `main.fsx` | Pipeline orchestration | `test`, `comps`, `compf` |

---

## Quick Start

### Prerequisites

```bash
# Check .NET installation
dotnet --version   # Requires 6.0 or later

# Install FsLex/FsYacc (only needed if modifying lexer/parser)
dotnet tool install --global FsLexYacc
```

### Clone & Build

```bash
git clone https://github.com/sabin147/Functional-Programming.git
cd Functional-Programming
dotnet build
```

### Run in F# Interactive

```bash
dotnet fsi
```

```fsharp
// Load the project
#load "main.fsx"

// Quick test - compiles, assembles, and executes
Main.test "5 + 3 * 2"

// Step-by-step pipeline
let ast = Parse.fromString "1 + 2"
let instructions = Compiler.compProg ast
let bytecode = Asm.asm instructions
let result = VM.exec bytecode   // 3

// Test from string or file
Main.comps "10 / 2"             // returns bytecode
Main.test "if 5 > 3 then 100 else 200"
```

### Example Programs

```fsharp
// Basic arithmetic
Main.test "2 + 3 * 4"                    // 14

// Boolean logic (short-circuiting)
Main.test "true && (write 99; false)"    // false (99 never printed)

// Conditional expression
Main.test "if 5 > 3 then 100 else 200"   // 100

// Function definition & call
Main.test @"
    let double x = x * 2
    double 21"                           // 42

// Multi-argument function
Main.test @"
    let add x y = x + y
    add 10 20"                           // 30
```

---

## Language Specification

### Types

| Type | Literals | Operations |
|------|----------|------------|
| `int` | `0`, `-5`, `42` | `+`, `-`, `*`, `/`, `%`, unary `-` |
| `bool` | `true`, `false` | `&&`, `||`, `==`, `!=`, `<`, `<=`, `>`, `>=` |
| `unit` | `()` | Return type of `write` and no-arg functions |

### Grammar (Simplified)

```
program    = let* expr
let        = "let" name "=" expr
expr       = int | bool | name
           | expr "+" expr | expr "*" expr 
           | expr "==" expr | expr "<" expr
           | "if" expr "then" expr "else" expr
           | name expr*                    // function call
           | "read" | "write" expr
           | "(" expr ")"
```

### Operator Precedence (highest to lowest)

| Precedence | Operators | Associativity |
|------------|-----------|---------------|
| 1 | unary `-` | Right |
| 2 | `*`, `/`, `%` | Left |
| 3 | `+`, `-` | Left |
| 4 | `==`, `!=`, `<`, `<=`, `>`, `>=` | Left |
| 5 | `&&` | Left |
| 6 | `||` | Left |

---

## Compilation Example

### Input Program

```
(5 + 3) * 2
```

### Phase 1: Parsing → AST

```
Mul(Add(Int 5, Int 3), Int 2)
```

### Phase 2: Compilation → Symbolic Instructions

| Step | Instruction | Stack After | Comment |
|------|-------------|-------------|---------|
| 1 | `IPUSH 5` | `[5]` | Push left constant |
| 2 | `IPUSH 3` | `[5, 3]` | Push right constant |
| 3 | `IADD` | `[8]` | 5 + 3 = 8 |
| 4 | `IPUSH 2` | `[8, 2]` | Push constant |
| 5 | `IMUL` | `[16]` | 8 * 2 = 16 |
| 6 | `IHALT` | `[16]` | Halt with result |

### Phase 3: Assembly → Bytecode

```
[273, 5, 273, 3, 289, 273, 2, 290, 257]
```

### Phase 4: VM Execution

```
Executed 6 instructions.
Result = 16
```

---

## Key Technical Implementations

### 1. Environment Management

The compiler tracks variable bindings using an **environment list** where the index represents stack offset:

```fsharp
// compExpr : expr -> env:int list -> instruction list
// 
// Example environment: env = [x; y; z] means:
//   - x at stack offset 0
//   - y at offset 1  
//   - z at offset 2
//
// Getting variable "y": find its index (1), generate ILOAD 1

let rec compExpr e env =
    match e with
    | Var name -> 
        let idx = List.findIndex (fun n -> n = name) env
        [ILOAD idx]
    | Let(name, rhs, body) ->
        compExpr rhs env           // compute rhs value
        @ compExpr body (name :: env)  // extend env, compile body
        @ [ISWAP; IPOP]           // clean up
```

### 2. Function Calling Convention

Each function call follows a **standardized sequence**:

```
Before call:  [argN, ..., arg1]  (N arguments on stack)

1. ICALL <arity> <address>
   - Saves return address
   - Jumps to function code
   
During function:
   - Arguments accessible via ILOAD (relative to base pointer)
   - Local bindings allocated on stack
   
Return from function:
   - Result left on stack
   - Arguments popped (callee cleanup)
   
After call:   [result]            (result replaces arguments)
```

### 3. Type System (Interpreter)

```fsharp
type Value = 
    | IntVal of int
    | BoolVal of bool
    | UnitVal

let add v1 v2 =
    match v1, v2 with
    | IntVal x, IntVal y -> IntVal (x + y)
    | _ -> failwith "Type error: expected int"

let eq v1 v2 =
    match v1, v2 with
    | IntVal x, IntVal y -> BoolVal (x = y)
    | BoolVal x, BoolVal y -> BoolVal (x = y)
    | UnitVal, UnitVal -> BoolVal true
    | _ -> BoolVal false
```

---

## Calling Convention

### Function Call Sequence

| Step | Instruction | Stack Before | Stack After | Responsibility |
|------|-------------|--------------|-------------|----------------|
| 1 | Push args (right-to-left) | `[ ]` | `[argN, ..., arg1]` | Caller |
| 2 | `ICALL n addr` | `[args...]` | `[result]` | VM (saves RA) |
| 3 | Function body executes | - | - | Callee |
| 4 | `IRET` | `[result]` | `[result]` | Callee (pops RA) |
| 5 | Cleanup (if needed) | `[result]` | `[result]` | Caller |

### Example: `add 10 20`

```
Initial:        []
IPUSH 20        [20]
IPUSH 10        [20, 10]
ICALL 2 addr    [] → then result pushed
; function body: ILOAD 0, ILOAD 1, IADD
; returns:       [30]
```

---

## Short-Circuit Evaluation

### Implementation of `&&`

`e1 && e2` compiles to:

```
code for e1                 // evaluate left operand
IJMPIF <false_label>        // if false, jump over e2
code for e2                 // evaluate right operand
ILAB <false_label>          // target for false branch
```

### Why This Matters

Without short-circuiting, `false && (1/0)` would divide by zero.  
With short-circuiting, the division is never evaluated.

### Example

```fsharp
// Input: false && (write "error"; true)
// Output: false (no "error" printed)
// 
// Compilation:
//   IPUSH 0          (false = 0)
//   IJMPIF label     (jump because false)
//   ... (write code) (SKIPPED!)
//   ILAB label       (continue here)
```

---

## Design Decisions

| Decision | Rationale | Trade-off |
|----------|-----------|-----------|
| Environment as `int list` | Simple, immutable, easy to extend | O(n) lookup (fine for small scopes) |
| Dynamic typing | Simpler VM, matches course focus | No compile-time type guarantees |
| First-order functions | Straightforward calling convention | No closures or higher-order functions |
| Stack-based VM | Simple to compile to, easy to reason about | Less efficient than register machines |
| Callee cleanup (`IPOP`) | Reduces code duplication | Callee must know argument count |
| Short-circuit via jumps | Correct semantics, explicit control flow | More instructions than naive approach |

---

## Troubleshooting

| Error | Likely Cause | Solution |
|-------|--------------|----------|
| `error FS0039: 'Parse' not defined` | Forgot to load main.fsx | `#load "main.fsx"` |
| `Stack overflow` | Recursive function without base case | Check function termination condition |
| `Type mismatch in comparison` | Comparing int with bool | Ensure both operands have same type |
| `IJMPIF jumps to wrong location` | Label offset miscalculation | Verify label generation in `compileAnd`/`compileOr` |
| `ICALL fails with arity mismatch` | Wrong number of arguments | Check function definition vs call site |
| `Variable not found in environment` | Variable out of scope | Check let-binding structure |
| `VM.exec returns wrong value` | Environment offset error | Verify `addOffset` function |

### Debugging Tips

```fsharp
// View AST
Parse.fromString "1 + 2" |> printfn "%A"

// View compiled instructions
Compiler.compProg ast |> printfn "%A"

// View bytecode
Asm.asm instructions |> printfn "%A"

// Single-step through pipeline
let ast = Parse.fromString "your code"
printfn "AST: %A" ast
let ins = Compiler.compProg ast
printfn "Instructions: %A" ins
VM.exec (Asm.asm ins)
```

---

## Building & Testing

### Build Project

```bash
dotnet build
```

### Run Full Test Suite

```bash
dotnet run -- test-all
```

### Manual Testing in F# Interactive

```fsharp
#load "main.fsx"

// Test compilation without execution
Main.comps "1 + 2"

// Full test with output
Main.test "if true then 42 else 0"

// Compare compiler vs interpreter
Main.test "factorial 5"
Main.testInterp "factorial 5"
```

### Partial Hand-ins (Course Requirements)

| Hand-in | Deadline | Features to Implement |
|---------|----------|----------------------|
| Part 1 | Week 4 | Constants `true`/`false`, operators `- * / %`, unary `-`, comparisons (`== != < <= > >=`), `if` expressions |
| Part 2 | Week 5 | `&&`, `||`, `read`, `write` |
| Part 3 | Week 6 | Multi-argument functions (0 or 2+ arguments) |

---

## Academic Context

This project was developed for the **Functional Programming and Language Implementations (FPLI)** course at Roskilde University (Spring 2026).

### Course Focus

- Principles of language engineering
- Structural recursion on abstract syntax
- Virtual machine architecture
- Compiler design for functional languages

### Key Concepts Demonstrated

| Concept | Implementation Location |
|---------|------------------------|
| Structural Recursion | `Compiler.fs` - `compExpr` function |
| Pattern Matching | All `match` expressions across codebase |
| Immutable Data | Environment as `int list`, AST as discriminated union |
| First-order Functions | Function call/return in `Compiler.fs` |
| Dynamic Typing | `Value` type in `Interpreter.fs` |

### Exam Topics Covered

| Topic | How Project Demonstrates |
|-------|-------------------------|
| Expressions and values | AST (`Expr`) and evaluation (interpreter) |
| Pattern matching | Every compiler case uses pattern matching |
| Functions and recursion | `let` bindings, function calls, tail recursion |
| Abstract syntax trees | `Syntax.fs` defines full AST |
| Stack-based VM | `VM.dll` executes bytecode on operand stack |
| Compiling variables | Environment tracking with stack offsets |
| Calling conventions | `ICALL` sequence with argument cleanup |

---

## Quick Reference

### VM Instruction Set

| Instruction | Stack Effect | Description |
|-------------|--------------|-------------|
| `IPUSH n` | `push n` | Push integer constant |
| `IPOP` | `pop` | Discard top value |
| `IADD` | `a, b → a+b` | Integer addition |
| `ISUB` | `a, b → a-b` | Integer subtraction |
| `IMUL` | `a, b → a*b` | Integer multiplication |
| `IDIV` | `a, b → a/b` | Integer division |
| `IREM` | `a, b → a%b` | Integer remainder |
| `INEG` | `a → -a` | Integer negation |
| `IEQ` | `a, b → a=b` | Equality |
| `ILT` | `a, b → a<b` | Less than |
| `IJMP addr` | - | Unconditional jump |
| `IJMPIF addr` | `cond →` | Jump if true (non-zero) |
| `ILAB addr` | - | Label (no-op) |
| `ILOAD n` | `push stack[n]` | Load from stack offset |
| `ICALL n addr` | `args → result` | Call function (n args) |
| `IRET` | `result` | Return from function |
| `ISWAP` | `a, b → b, a` | Swap top two |
| `IHALT` | - | Stop execution |

### Compiler Functions

| Function | Signature | Purpose |
|----------|-----------|---------|
| `compExpr` | `Expr → int list → Asm.inst list` | Compile expression with environment |
| `compProg` | `program → Asm.inst list` | Compile entire program |
| `addOffset` | `int → int list → int list` | Add n to all environment offsets |
| `findVar` | `string → int list → int` | Find variable's stack offset |

### Environment Helper Functions

```fsharp
// Empty environment
let emptyEnv = []

// Extend environment (add new binding at front)
let extend env name = name :: env

// Find offset (0 = top of stack)
let lookup name env = List.findIndex (fun n -> n = name) env
```

---

## Glossary

| Term | Definition |
|------|-------------|
| **AST** | Abstract Syntax Tree – tree representation of source code after parsing |
| **Discriminated Union** | F# type with multiple labelled cases (e.g., `Int of int \| Add of Expr * Expr`) |
| **Pattern Matching** | Deconstructing values and branching by their structure using `match` |
| **Calling Convention** | Protocol for function calls (argument order, who cleans up, return address handling) |
| **Short-Circuit** | `&&` and `||` that skip evaluating the second operand when the first determines the result |
| **Environment** | Mapping from variable names to stack offsets (compile-time) |
| **Stack Frame** | Region of stack containing arguments, locals, and return address for a function call |
| **First-order Function** | Function that cannot be passed as argument or returned (contrast with higher-order) |
| **Dynamic Typing** | Type checking performed at runtime (vs static typing at compile time) |
| **Bytecode** | Compact representation of instructions executed by the VM |
| **Instruction** | Single operation in the VM's instruction set (e.g., `IADD`, `IPUSH`) |

---

## References

1. Hansen, M.R. & Rischel, H. *Functional Programming Using F#*. Cambridge University Press. ISBN 978-1-107-68406-5. Chapters 1-6.

2. Rhiger, M. (2026). *Functional Language Implementations* (Course Notes). Roskilde University.

3. Microsoft. *F# Documentation*. https://docs.microsoft.com/en-us/dotnet/fsharp/

4. FsLex/FsYacc Project. https://github.com/fsprojects/FsLexYacc

---

## Acknowledgments

- **Morten Rhiger** – Course instructor, for guidance on compiler design and VM architecture
- **Functional Programming and Language Implementations (FPLI) F2026** – Course staff and fellow students
- FsLex/FsYacc contributors – Parser generator tools used for lexing and parsing

---

## Contact

**Author:** sabin147  
**Course:** Functional Programming and Language Implementations (FPLI) – Spring 2026  
**Institution:** Roskilde University  
**GitHub:** [github.com/sabin147/Functional-Programming](https://github.com/sabin147/Functional-Programming)

---

## License

This project was developed as coursework for the FPLI course at Roskilde University.  
For academic use only. Please contact the course instructor before reusing or redistributing.
```

---

