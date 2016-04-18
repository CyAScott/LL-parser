using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLParser
{
    public class LlParser
    {
        private readonly List<TreeBranch> derivationTreeBranches;
        private readonly char[] nonTerminals;
        private readonly char[] terminals;
        private void buildTree(GrammarRules grammarRules, 
            TreeBranch parentBranch = null, 
            GrammarRules startingRule = null, int? startingRuleIndex = null)
        {
            for (var index = 0; index < grammarRules.Rules.Count; index++)
            {
                var rule = new GrammarRuleInfo(grammarRules.Name, index);
                var branch = parentBranch == null ? new TreeBranch(rule) : new TreeBranch(parentBranch, rule);

                for (var ruleIndex = 0; ruleIndex < grammarRules.Rules[index].Length; ruleIndex++)
                {
                    var c = grammarRules.Rules[index][ruleIndex];

                    if (terminals.Contains(c))
                    {
                        branch.Terminals.Append(c);
                        if (branch.Terminals.Length == K)
                        {
                            branch.Rule = (startingRule ?? grammarRules).Rules[startingRuleIndex ?? index];

                            var branchKey = branch.ToString();
                            if (derivationTreeBranches.All(tree => tree.ToString() != branchKey))
                                derivationTreeBranches.Add(branch);
                            break;
                        }
                    }
                    else if (nonTerminals.Contains(c))
                    {
                        if (branch.BranchHeight == MaxTreeHeight)
                        {
                            throw new StackOverflowException("The CFG is too complex to parse.");
                        }

                        var childRule = Language[c];

                        //branch.Rule.Append(c);

                        buildTree(childRule, branch,
                            startingRule ?? grammarRules,
                            startingRuleIndex ?? index);

                        //No need to process any more because this rule did not produce
                        //a string of terminals equal to k before needing a another nested
                        //rule. All possible nested rules were explored, and if any of
                        //those produced a string of terminals equal to k then they were
                        //add to the tree branches.
                        break;
                    }
                    else
                    {
                        //this should never happen, but let's throw an error just in case
                        throw new InvalidProgramException("Unknown alphabet: " + c);
                    }
                }
            }
        }
        private string getKey(GrammarRuleInfo info, string input)
        {
            return String.Format("{0} -> {1}", info.Name, input);
        }
        private string getKey(char path, string input)
        {
            return String.Format("{0} -> {1}", path, input);
        }

        public LlParser(Language language, int k, int maxTreeHeight = 255)
        {
            if (k < 1) throw new IndexOutOfRangeException("k");
            if (language == null) throw new ArgumentNullException("language");

            Language = language;
            MaxTreeHeight = maxTreeHeight;
            nonTerminals = Language.NonTerminals.ToCharArray();
            derivationTreeBranches = new List<TreeBranch>();
            terminals = Language.Terminals.ToCharArray();

            //build derivation trees
            K = k;
            //for (K = 1; K <= k; K++)
                foreach (var grammar in Language)
                {
                    buildTree(grammar);
                }
            //K--;

            //find ambiguous rules
            var permutations = derivationTreeBranches
                .Select(branch => String.Format("{0}[{1}] -> {2}", branch.FirstRule.Name, branch.FirstRule.RuleIndex, branch.Terminals)).ToArray();
            var ambiguousInputs = permutations
                .Where(permutation => permutations.Count(subPermutation => subPermutation == permutation) > 1)
                .Distinct()
                .ToArray();
            if (ambiguousInputs.Length > 0)
                throw new ArgumentException(String.Format(
                    "The language has ambiguous grammar rules with a k size of {0}. " +
                    "The following rules produce the same inputs: {1}",
                    K,
                    String.Join(", ", ambiguousInputs)));

            //convert derivation trees into a parsing hash table
            LlParsingTable = new Dictionary<string, string>();
            foreach (var path in derivationTreeBranches)
            {
                var key = getKey(path.FirstRule, path.Terminals.ToString());
                if (LlParsingTable.ContainsKey(key))
                {
                    if (LlParsingTable[key] != path.Rule)
                        throw new ArgumentException("There is an ambiguous rule.");
                }
                else
                {
                    LlParsingTable.Add(key, path.Rule);
                }
            }
            //LlParsingTable = derivationTreeBranches
            //    .Distinct(path => getKey(path.FirstRule, path.Terminals.ToString()))
            //    .ToDictionary(path => getKey(path.FirstRule, path.Terminals.ToString()), path => path.Rule.ToString());
        }
        public Dictionary<string, string> LlParsingTable { get; private set; }
        public IEnumerable<Tuple<char, string>> Parse(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                //try the special case of an empty string input

                var emptyStringPath = derivationTreeBranches.FirstOrDefault(path => path.Terminals.Length == 0 && path.FirstRule.Name == Language.Start.Name);

                if (emptyStringPath == null) throw new ArgumentException("This lagnauge does not allow for an empty string input.");

                foreach (var path in emptyStringPath.Path)
                {
                    yield return new Tuple<char, string>(path.Name, Language.Start.Name.ToString());
                }

                yield break;
            }

            var inputCount = input.Length;
            var derivation = new StringBuilder(Language.Start.Name.ToString());
            var readInput = new StringBuilder();
            var symbolStack = new List<char>
            {
                Language.Start.Name
            };
            var inputStream = new List<char>(input);

            while (inputStream.Count > 0)
            {
                var token = inputStream.PeekFromStart(K);
                var symbol = symbolStack.PullFromStart();

                if (nonTerminals.Contains(symbol))
                {
                    var key = getKey(symbol, token);

                    string rule;
                    if (!LlParsingTable.TryGetValue(key, out rule))
                        throw new ArgumentException("Unable to parse input starting at: " + (inputCount - inputStream.Count));

                    symbolStack.PushToStart(rule);

                    yield return new Tuple<char, string>(symbol, derivation.ToString());

                    derivation.Clear();
                    derivation.Append(readInput);
                    derivation.Append(symbolStack.ToArray());
                }
                else if (terminals.Contains(symbol))
                {
                    if (token.First() == symbol)
                    {
                        //do nothing because it is an expected terminal
                        readInput.Append(symbol);

                        inputStream.PullFromStart();
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected terminal in input string starting at: " + (inputCount - inputStream.Count));
                    }
                }
            }
        }
        public Language Language { get; private set; }
        public int K { get; private set; }
        public int MaxTreeHeight { get; set; }
    }
    public class TreeBranch
    {
        public TreeBranch(GrammarRuleInfo rule)
        {
            Path = new List<GrammarRuleInfo>
            {
                rule
            };
            Terminals = new StringBuilder();
        }
        public TreeBranch(TreeBranch copy, GrammarRuleInfo rule)
        {
            BranchHeight = copy.BranchHeight + 1;
            Path = new List<GrammarRuleInfo>(copy.Path)
            {
                rule
            };
            Terminals = new StringBuilder(copy.Terminals.ToString());
        }
        public List<GrammarRuleInfo> Path { get; set; }
        public GrammarRuleInfo FirstRule
        {
            get
            {
                return Path.First();
            }
        }
        public StringBuilder Terminals { get; set; }
        public int BranchHeight { get; set; }
        public string Rule { get; set; }

        public override string ToString()
        {
            return Path.Count == 0 ? "Unknown" : String.Format("{0},{1} -> {2}", Terminals, FirstRule.Name, Rule);
        }
    }
    public class GrammarRuleInfo
    {
        public GrammarRuleInfo(char name, int ruleIndex)
        {
            Name = name;
            RuleIndex = ruleIndex;
        }
        public char Name { get; private set; }
        public int RuleIndex { get; private set; }
        public override string ToString()
        {
            return String.Format("{0}[{1}]", Name, RuleIndex);
        }
    }
}
