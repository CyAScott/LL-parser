using System;
using System.Text.RegularExpressions;

namespace LLParser.ConsoleApp
{
    public static partial class Program
    {
        public static Language GetLanguage()
        {
            string line;
            var language = new Language();

            do
            {
                WriteLines(
                    "Enter a grammer rule in the format: <Non-Terminal Char>=<Grammar Rules Delimited by '|'>",
                    "For example: S=a|b",
                    "When you are done typing the rule you can press enter.");

                line = ReadLine();

                try
                {
                    if (!Regex.IsMatch(line, @"^\s*\S\s*=.*$")) throw new ArgumentException("The input is invalid.");

                    var name = line.TrimStart()[0];

                    language[name] = new GrammarRules(name, line.TrimStart().Substring(1).TrimStart().Substring(1));

                    WriteLines("The current language is:");

                    ShowValue(language);

                    WriteLines("Do you want to enter another grammar rule (yes or no)?");

                    line = ReadLine();
                }
                catch (Exception error)
                {
                    line = null;
                    ShowError(error);
                }
            }
            while (String.IsNullOrWhiteSpace(line) || line.IndexOf("n", StringComparison.OrdinalIgnoreCase) == -1);

            do
            {
                WriteLines("Enter the non-terminal character for the starting node.");

                line = ReadLine();

                try
                {
                    if (!Regex.IsMatch(line, @"^\s*\S\s*$")) throw new ArgumentException("The input is invalid.");

                    var rule = language[line.Trim()[0]];

                    if (rule == null) throw new ArgumentException("No rule was found.");

                    language.Start = rule;
                }
                catch (Exception error)
                {
                    ShowError(error);
                }
            }
            while (language.Start == null);

            return language;
        }
    }
}
