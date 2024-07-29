# An implementation of Lox from the book Crafting Interpreters in C#
## Added Features
- Ternary operator
- Static methods
- Getters
- Anonymous functions
- i++ and i-- operator (returns updated value)
- break statements
- An exit function that takes in an exit value
- Multiline comments
- Expression support in the REPL
- Allow for strings to be added to non string types ie: ("1" + 1 = "11")
## Changes
- Disallow variable redeclaration in the same scope (doesn't include child scopes)
- An array is used instead of a hashmap to store values in the Environment class
- Division by zero error
