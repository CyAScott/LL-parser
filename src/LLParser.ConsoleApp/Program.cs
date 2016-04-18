using System;
using System.Collections.Generic;

namespace LLParser.ConsoleApp
{
    public static partial class Program
    {
        static Program()
        {
            Console.BufferHeight = 1024;
            Console.BufferWidth = 1024;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string ReadLine()
        {
            Console.ForegroundColor = ConsoleColor.White;
            var returnValue = Console.ReadLine();
            Console.WriteLine();
            return returnValue;
        }
        public static void WriteLines(params string[] lines)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var line in lines) Console.WriteLine(line);
            Console.WriteLine();
        }
        public static void ShowError(Exception error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error.Message);
            Console.WriteLine();
        }
        public static void ShowValue(object value)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(value);
            Console.WriteLine();
        }
        public static void Main()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[LL Parser Console App]");
                Console.WriteLine();

                var language = GetLanguage();

                WriteLines("The language is:");
                ShowValue(language);

                int k;
                do
                {
                    WriteLines("Enter the number of tokens to use for the parser.");
                    if (!Int32.TryParse(ReadLine(), out k) || k < 1)
                    {
                        ShowError(new ArgumentException("Invalid input."));
                    }
                }
                while (k < 1);

                WriteLines("Creating the parsing table.");

                var parsingTree = new LlParser(language, k);
                var rules = new List<char>();
                var derivation = new List<string>();

                WriteLines("Enter a string to parse. Press enter when done.");
                var input = ReadLine();
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var rule in parsingTree.Parse(input))
                {
                    rules.Add(rule.Item1);
                    derivation.Add(rule.Item2);

                    Console.WriteLine("Processing Rule: {0}", rule.Item1);
                }
                Console.WriteLine();

                WriteLines("Done.", "A total of " + rules.Count + " rules processed.",
                    "Rules Followed:", "\t" + String.Join(", ", rules),
                    "Leftmost Derivations:", "\t" + String.Join(" -> ", derivation) + " -> " + input);
            }
            catch (Exception error)
            {
                ShowError(error);
            }

            Console.ReadLine();
        }
    }
}
