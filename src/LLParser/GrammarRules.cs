using System;
using System.Collections.Generic;
using System.Linq;

namespace LLParser
{
    public class GrammarRules
    {
        public bool Equals(GrammarRules obj)
        {
            return !ReferenceEquals(obj, null) && 
            (
                ReferenceEquals(obj, this) ||
                obj.Rules == Rules ||
                (
                    obj.Rules != null &&
                    Rules != null &&
                    obj.Rules.All(rule => Rules.Contains(rule)) &&
                    Rules.All(rule => obj.Rules.Contains(rule))
                )
            );
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as GrammarRules);
        }
        public override int GetHashCode()
        {
            return Rules == null || Rules.Count == 0 ? 0 :
                Rules
                    .Select(rule => rule.GetHashCode())
                    .Aggregate((rulehashCode, returnValue) => rulehashCode ^ returnValue);
        }
        public override string ToString()
        {
            return String.Format("{0} -> {1}", Name, String.Join("|", Rules));
        }
        public static bool operator ==(GrammarRules a, GrammarRules b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(GrammarRules a, GrammarRules b)
        {
            return !Equals(a, b);
        }

        public GrammarRules(char name, string rules)
            : this(name, rules.Split('|'))
        {
        }
        public GrammarRules(char name, string firstRule, string secondRule, params string[] restOfTheRules)
            : this(name, new[] { firstRule, secondRule }.Concat(restOfTheRules))
        {
        }
        public GrammarRules(char name, IEnumerable<string> rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            Name = name;
            Rules = new List<string>(rules.Select(rule => rule ?? "").Distinct()).AsReadOnly();
        }

        public char Name { get; private set; }
        public IReadOnlyList<string> Rules { get; private set; }
    }
}
