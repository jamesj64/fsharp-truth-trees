# fsharp-truth-trees
Simple parser and tree proof generator for classical propositional logic

## User Guide
### Parsing
1. To parse a single formula from a single string, use `singleParse` of type `string -> Formula`.
2. To parse a list of formulas from a single string, each separated by a new line, use either `splitPremisesChar` or `splitPremisesUnchar` both of type `string -> []<Formula>`. Use `splitPremisesChar` if you want improperly formatted substrings to be ignored. Ex: in "P \n Q ⇒ R S \n (P ∧ Q) ⇒ R", "Q ⇒ R S" will be ignored. Use `splitPremisesUnchar` if you want the function to fail if not all of the substrings are properly formatted.
3. Strings to be parsed should be in infix notation. Several commonly used versions of each of the logical symbols are supported. All of the binary, truth-functional operators are right-associative, and they follow the conventional precedence rules. Atomic formulas should be composed of only letters. The only other non-logical symbols recognized are spaces and "\n". Spaces are ignored, and "\n" is treated as a delimeter for substrings which are intended to be formulas. For example, the result of the application of `splitPremisesChar` to "P \n P ⇒ Q \n Q" is `[Atom "P"; Implies(Atom "P", Atom "Q); Atom "Q"]`.
### Tree Generation
1. Use `createTree` of `List<Formula> -> Tree` if you have already parsed the formulas with which you wish to build a tree. The tree generated is effectively a proof that the formulas inputted are consistent, if it is open, and if it is closed, then it is a proof that the formulas inputted are inconsistent.
2. There are built-in functions such as `bTree` and `prove` of `string -> Tree` which take a string and from it generate a tree proof. `bTree` is good if you know the string from which you are generating a tree proof corresponds to a list of formulas of length 1. Otherwise, you should use `prove`.
3. Use `checkTree` of type `Tree -> bool` to determine whether a tree is closed, and thus whether the list of formulas used to generate said tree is inconsistent. `checkTree` will return true/false just in case the inputted tree is closed/open.
4. `prove` of `string -> Tree` parses each of the substrings, delimeted by "\n", and generates a tree from the resulting formulas. However, the formula from the last substring will be negated. Therefore, the application of `prove` to "P \n P ⇒ Q \n Q" will result in a closed tree, as the list of formulas `[Atom "P"; Implies(Atom "P", Atom "Q"); Not(Atom "Q")]` is inconsistent. This is useful if you want to determine whether an "argument" is valid.

## Additional functionality
1. The `toJson` function of type `Tree -> string` will generate json code given a tree.
2. The `prettyPrint` and `uglyPrint` functions both of type `Formula -> string` will return the string corresponding to the inputted formula with pretty symbols and ugly symbols respectively.
3. The `format` function of type `string -> string` will take a string such as "P -> Q -> P" and return the string "P ⇒ Q ⇒ P". It replaces all of the logical symbols with those used in parsing and in the `prettyPrint` function
