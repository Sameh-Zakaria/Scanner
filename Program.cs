using System;
using System.Collections.Generic;

class RecursiveDescentParser
{
    static Dictionary<string, List<string>> grammar = new Dictionary<string, List<string>>();
    static Stack<string> parseStack = new Stack<string>();
    static string input;
    static int pointer;

    static void Main()
    {
        Console.WriteLine("\uD83D\uDC49 Recursive Descent Parsing For Following Grammar \uD83D\uDC49");

        while (true)
        {
            grammar.Clear();

            // Input Grammar
            Console.WriteLine("\n\uD83D\uDC47 Grammars \uD83D\uDC47");
            InputGrammar();

            if (!IsSimpleGrammar())
            {
                Console.WriteLine("The Grammar isn't simple.\nTry again.");
                continue;
            }

            while (true)
            {
                parseStack.Clear();
                pointer = 0;

                Console.Write("\nEnter the string to be checked: ");
                input = Console.ReadLine();

                Console.WriteLine($"\nThe input String: [{string.Join(", ", input.ToCharArray())}]");

                parseStack.Push("S"); // Start with the start symbol S.

                if (Parse())
                {
                    Console.WriteLine($"Stack after checking: []");
                    Console.WriteLine($"The rest of unchecked string: []");
                    Console.WriteLine("Your input String is Accepted.");
                }
                else
                {
                    string remainingInput = pointer < input.Length ? input.Substring(pointer) : "";
                    Console.WriteLine($"Stack after checking: [{string.Join(" ", parseStack.ToArray())}]");
                    Console.WriteLine($"The rest of unchecked string: [{remainingInput}]");
                    Console.WriteLine("Your input String is Rejected.");
                }

                // Menu
                Console.WriteLine("\n========================================");
                Console.WriteLine("1-Another Grammar.\n2-Another String.\n3-Exit");
                Console.Write("Enter your choice: ");
                int choice = int.Parse(Console.ReadLine());
                if (choice == 1) break;
                else if (choice == 3) return;
            }
        }
    }

    // Input Grammar Rules
    static void InputGrammar()
    {
        for (int i = 0; i < 2; i++)
        {
            string nonTerminal = i == 1 ? "B" : "S"; // Second non-terminal is 'B', first is 'S'
            Console.Write($"Enter rule number 1 for non-terminal '{nonTerminal}': ");
            string rule1 = Console.ReadLine();
            Console.Write($"Enter rule number 2 for non-terminal '{nonTerminal}': ");
            string rule2 = Console.ReadLine();

            grammar[nonTerminal] = new List<string> { rule1, rule2 };
        }
    }

    // Validate Grammar Simplicity
    static bool IsSimpleGrammar()
    {
        foreach (var nonTerminal in grammar.Keys)
        {
            var rules = grammar[nonTerminal];

            // Check if all rules for this non-terminal are of the form A -> aX (X can be empty)
            // and have unique first terminals
            HashSet<char> firstTerminals = new HashSet<char>();
            foreach (var rule in rules)
            {
                if (rule.Length == 0)
                {
                    return false; // Empty rule is not allowed (since X cannot be empty)
                }

                char firstSymbol = rule[0];
                if (!char.IsLower(firstSymbol))
                {
                    return false; // First symbol must be a lowercase letter (terminal)
                }

                if (!firstTerminals.Add(firstSymbol))
                {
                    return false; // Duplicate first terminal for this non-terminal
                }
            }
        }

        return true;
    }
    // Recursive Parsing Function
    static bool Parse()
    {
        while (parseStack.Count > 0)
        {
            string top = parseStack.Pop();

            // Check terminal symbol
            if (!grammar.ContainsKey(top))
            {
                if (pointer < input.Length && input[pointer].ToString() == top)
                {
                    pointer++; // Move input pointer forward
                }
                else
                {
                    return false; // Terminal mismatch
                }
            }
            else
            {
                bool matched = false;
                foreach (string rule in grammar[top])
                {
                    Stack<string> tempStack = new Stack<string>(parseStack);
                    int tempPointer = pointer;

                    // Push rule in reverse order
                    for (int i = rule.Length - 1; i >= 0; i--)
                    {
                        parseStack.Push(rule[i].ToString());
                    }

                    if (Parse()) // Recursive call
                    {
                        matched = true;
                        break;
                    }

                    // Backtrack if rule fails
                    parseStack = new Stack<string>(tempStack);
                    pointer = tempPointer;
                }
                if (!matched) return false;
            }
        }

        return pointer == input.Length; // Successful parse
    }
}