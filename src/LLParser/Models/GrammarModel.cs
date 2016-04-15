using System;
using System.Collections.Generic;
using System.Linq;

namespace LLParser.Models
{
    public class GrammarModel
    {
        public bool Equals(GrammarModel obj)
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
            return Equals(obj as GrammarModel);
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
        public static bool operator ==(GrammarModel a, GrammarModel b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(GrammarModel a, GrammarModel b)
        {
            return !Equals(a, b);
        }

        public GrammarModel(char name, string rules)
            : this(name, rules.Split('|'))
        {
        }
        public GrammarModel(char name, string firstRule, string secondRule, params string[] restOfTheRules)
            : this(name, new[] { firstRule, secondRule }.Concat(restOfTheRules))
        {
        }
        public GrammarModel(char name, IEnumerable<string> rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            Name = name;
            Rules = new HashSet<string>(rules.Select(rule => rule ?? ""));
        }

        public char Name { get; set; }
        public HashSet<string> Rules { get; set; }
    }
}
