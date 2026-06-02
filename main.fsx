#r "nuget: FsLexYacc.Runtime, 10.2.0"
#r "vm.dll"
#r "asm.dll"
#load "syntax.fs" "parser.fs" "lexer.fs" "parse.fs" "compiler.fs"

// Add more F# code here

let comps s = Asm.asm (Compiler.compProg (Parse.fromString s));;
let compf f = Asm.asm (Compiler.compProg (Parse.fromFile f));;

let testWithMessage msg f x =
  printf "[%s]\n" msg
  let result = f x
  result

let test s = 
  printf "Testing program:\n"
  printf "      %s\n" s
  let ast  = testWithMessage "Parse.fromString" Parse.fromString s
  let trg  = testWithMessage "Compiler.compProg" Compiler.compProg ast
  let code = testWithMessage "Asm.asm" Asm.asm trg 
  let v    = testWithMessage "VM.exec" VM.exec code 
  printf "  Result = %d\n\n" v 
