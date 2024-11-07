using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Scanner
{
    // Define token types with regular expressions
    private static readonly List<(string Type, string Pattern)> TokenSpecifications = new List<(string, string)>
    {
        ("KEYWORD", @"\b(int|float|return|if|else|for|while|do|break|continue|void|char|double)\b"),
        ("IDENTIFIER", @"[a-zA-Z_]\w*"),
        ("NUMBER", @"\b\d+(\.\d*)?\b"),
        ("OPERATOR", @"[+\-*/%=<>!&|~^]"),
        ("SEPARATOR", @"[{}()[\],;.]"),
        ("STRING", @"""[^""\\]*(?:\\.[^""\\]*)*"""),  // Handle escaped characters within strings
        ("CHAR", @"'[^'\\]*(?:\\.[^'\\]*)*'"),       // Handle escaped characters within chars
        ("COMMENT", @"//.*?$|/\*.*?\*/"),            // Single-line and multi-line comments
        ("WHITESPACE", @"\s+")
    };

    // Compile the regular expression pattern
    private static readonly Regex TokenRegex = new Regex(
        string.Join("|", TokenSpecifications.ConvertAll(spec => $"(?<{spec.Type}>{spec.Pattern})")),
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline
    );

    public static List<(string Type, string Value)> Scan(string code)
    {
        var tokens = new List<(string Type, string Value)>();
        var matches = TokenRegex.Matches(code);

        foreach (Match match in matches)
        {
            foreach (var spec in TokenSpecifications)
            {
                if (match.Groups[spec.Type].Success)
                {
                    if (spec.Type != "WHITESPACE") // Skip whitespace
                    {
                        tokens.Add((spec.Type, match.Value));
                    }
                    break;
                }
            }
        }

        return tokens;
    }
}

class Program
{
    static void Main()
    {
        string code = @"
            int main() {
                // This is a comment
                int x = 10;
                float y = 20.5;
                printf(""Hello, World!"");
                return 0;
            }
        ";

        var tokens = Scanner.Scan(code);

        foreach (var token in tokens)
        {
            Console.WriteLine($"{token.Type}: '{token.Value}'");
        }
    }
}
