# LL-parser
Final project for COT 4210 Automata TheoFormal Languages

# Team Members
Cy Scott

# Compile and Run

This project was made with Visual Studio 2013 (VS) and should work on any version of VS after that one. Simply open /src/FinalProject.sln with VS then build and run the LLParser project. You can compile with mono using `xbuild /p:Configuration=Release src/FinalProject.sln` then run the program with
`mono src/LLParser/bin/Release/LLParser.exe`.

# Exmaple Output of Program
Enter a grammer rule in the format: (Non-Terminal Char) = (Grammar Rules Delimited by '|')  
For example: S=a|b  
When you are done typing the rule you can press enter.  

####S=a|(S+S)

The current language is:

`V = S`  
`Σ = ()+a`  
`R =`  
` S -> a|(S+S)`  
`S =`  

Do you want to enter another grammar rule (yes or no)?

####no

Enter the non-terminal character for the starting node.

####S

The language is:

`V = S`  
`Σ = ()+a`  
`R =`  
` S -> a|(S+S)`  
`S = S -> a|(S+S)`  

Enter the number of tokens to use for the parser.

####1

Creating the parsing table.

Enter a string to parse. Press enter when done.

####((a+a)+a)

Processing Rule: S  
Processing Rule: S  
Processing Rule: S  
Processing Rule: S  
Processing Rule: S  

Done.  
A total of 5 rules processed.  
Rules Followed:  
`S, S, S, S, S`  
Leftmost Derivations:  
`S -> (S+S) -> ((S+S)+S) -> ((a+S)+S) -> ((a+a)+S) -> ((a+a)+a)`  
