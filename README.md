# Functional Programming Language Implementation : Compiler & Virtual Machine , Interpreter

A complete functional programming language compiler and virtual machine implementation developed for the Functional Programming and Language Implementation (FPLI) course. This project bridges high-level source code with low-level machine execution through a robust pipeline including parsing, compilation, and stack-based interpretation.

## 🚀 Overview

This project implements a custom functional language with a primary focus on:

* **Compilation:** Converting abstract syntax trees (AST) into executable stack-machine instructions.
* **Virtual Machine:** A custom VM architecture for executing the compiled bytecode.
* **Interpreter:** A dynamically-typed runtime environment for direct expression evaluation (Part B).
* **Type Safety:** Implementation of dynamic type checking and runtime validation.

## 🛠 Features

* **Abstract Syntax Tree (AST):** Defined via recursive Discriminated Unions.
* **Recursive Traversal:** Compiler uses pattern matching to perform depth-first traversal of the AST.
* **Advanced Control Flow:** Handles short-circuiting logic (`&&`, `||`) using jump labels (`ILAB`, `IJMP`).
* **Calling Conventions:** Manages stack frames, return addresses, and argument cleanup for multi-argument functions.
* **Dynamic Typing:** A robust `value` type system with runtime type-tag checking.

## 📁 Project Structure

* `/Syntax.fs` – Language definition and AST structure.
* `/Compiler.fs` – Core compilation logic and tree-to-stack transformations.
* `/VM.dll` – The stack machine execution engine.
* `/Interpreter.fs` – Dynamic evaluator for direct code execution.
* `/Asm.dll` – Assembler definition and instruction set.
*  `/Functional Programming and Lanuage Implementation.pdf` – Concise presentation of the prject.

## ⚙️ How to Build & Run

1. **Clone the repository:**
`git clone https://github.com/sabin147/Functional-Programming.git`
2. **Load the environment:**
Open the project in your F# environment (e.g., VS Code with Ionide or `dotnet fsi`).
3. **Run the compiler:**
```fsharp
#load "main.fsx"
// Use the Parse, Compile, and Execute pipeline:
let ast = Parse.fromString "your_program_here"
let code = Compiler.compProg ast
VM.exec (Asm.asm code)

```



## 🏗 Key Technical Concepts

* **Environment Management:** We maintain stack integrity by tracking offsets at both compile-time (via environment lists) and run-time (via stack manipulation).
* **Short-Circuit Evaluation:** Implemented via conditional jump (`IJMPIF`) logic to optimize boolean operations.
* **Calling Convention:** A precise sequence of `ICALL`, `ISWAP`, and `IPOP` instructions ensures seamless function execution and stack cleanup.

## 🎓 Academic Context

This project was developed for the FPLI course, focusing on the principles of language engineering, structural recursion, and virtual machine architecture.

---
