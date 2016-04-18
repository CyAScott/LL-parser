using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LLParser
{
    public class Language : IEnumerable<GrammarRules>
    {
        IEnumerator<GrammarRules> IEnumerable<GrammarRules>.GetEnumerator()
        {
            return grammarRules.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return grammarRules.GetEnumerator();
        }

        private GrammarRules start;
        private readonly HashSet<GrammarRules> grammarRules;

        public override string ToString()
        {
            return String.Format(
                "V = {1}{0}" +
                "Σ = {2}{0}" +
                "R = {3}{0}" +
                "S = {4}",
                Environment.NewLine,
                NonTerminals,
                Terminals,
                grammarRules.Count == 0 ? "No Rules" : 
                    Environment.NewLine + "\t" + String.Join(Environment.NewLine + "\t", grammarRules),
                Start);
        }

        public Language()
        {
            grammarRules = new HashSet<GrammarRules>();
        }
        public GrammarRules this[char name]
        {
            get
            {
                return grammarRules.FirstOrDefault(rule => rule.Name == name);
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (grammarRules.Any(rule => rule.Name == value.Name)) throw new DuplicateNameException("Duplicate rule name.");
                if (grammarRules.Contains(value)) throw new DuplicateNameException("Duplicate rule.");
                grammarRules.Add(value);
            }
        }

        public GrammarRules Start
        {
            get
            {
                return start;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                
                var rule = this[value.Name];
                if (rule == null) throw new ArgumentException("The provided rule was not found.");

                if (rule != value) throw new ArgumentException("The provided rule does not match any rule in this language.");

                start = rule;
            }
        }
        public string NonTerminals
        {
            get
            {
                return new string(grammarRules.Select(rule => rule.Name)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToArray());
            }
        }
        public string Terminals
        {
            get
            {
                var nonTerminals = NonTerminals;
                return new string(grammarRules.SelectMany(rule => rule.Rules)
                    .SelectMany(rule => rule.ToCharArray())
                    .Where(c => nonTerminals.IndexOf(c) == -1)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToArray());
            }
        }
    }
}
