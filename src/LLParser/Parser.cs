using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLParser
{
    public class TreeRoot
    {
        private readonly List<TreePath> paths;
        private readonly char[] nonTerminals;
        private readonly char[] terminals;
        private void buildTree(GrammarRules grammarRules, TreePath parentNode = null)
        {
            for (var index = 0; index < grammarRules.Rules.Count; index++)
            {
                var newNode = parentNode == null ? new TreePath(this) : new TreePath(parentNode);

                newNode.Path.Add(new GrammarRulePath(grammarRules.Name, index));

                foreach (var c in grammarRules.Rules[index])
                {
                    if (terminals.Contains(c))
                    {
                        newNode.Text.Append(c);
                        if (newNode.Text.Length == K)
                        {
                            paths.Add(newNode);
                            break;
                        }
                    }
                    else if (nonTerminals.Contains(c))
                    {
                        if (newNode.Level == MaxRecursionSearch)
                        {
                            StackOverFlowInSearch = true;
                            break;
                        }
                        buildTree(Language[c], newNode);
                    }
                    else
                    {
                        throw new InvalidProgramException("Unknown alphabet: " + c);
                    }
                }
            }
        }
        private string getKey(GrammarRulePath path, string input)
        {
            return String.Format("{0} -> {1}", path.Name, input);
        }
        private string getKey(char path, char input)
        {
            return String.Format("{0} -> {1}", path, input);
        }

        public TreeRoot(Language language, int k, int maxRecursionSearch = 255)
        {
            if (k < 1) throw new IndexOutOfRangeException("k");
            if (language == null) throw new ArgumentNullException("language");

            K = k;
            Language = language;
            LlParsingTable = new Dictionary<string, GrammarRulePath>();
            MaxRecursionSearch = maxRecursionSearch;
            nonTerminals = Language.NonTerminals.ToCharArray();
            paths = new List<TreePath>();
            StackOverFlowInSearch = false;
            terminals = Language.Terminals.ToCharArray();
            
            foreach (var grammar in Language)
            {
                buildTree(grammar);
            }

            var permutations = paths.Select(path => String.Format("{0}[{1}] -> {2}", path.Path[0].Name, path.Path[0].RuleIndex, path.Text)).ToArray();
            var ambiguousInputs = permutations
                .Where(permutation => permutations.Count(subPermutation => subPermutation == permutation) > 1)
                .Distinct()
                .ToArray();
            if (ambiguousInputs.Length > 0)
                throw new ArgumentException(String.Format(
                    "The language has ambiguous grammar rules with a k size of {0} and max recursion search depth of {1}. " +
                    "The following inputs have multiple grammer rule paths: {2}",
                    K,
                    MaxRecursionSearch,
                    String.Join(", ", ambiguousInputs)));

            foreach (var path in paths)
            {
                var start = path.Path.First();

                LlParsingTable.Add(getKey(start, path.Text.ToString()), start);
            }
        }
        public Dictionary<string, GrammarRulePath> LlParsingTable { get; private set; }
        public IEnumerable<Tuple<GrammarRulePath, string>> Parse(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                var emptyStringPath = paths.FirstOrDefault(path => path.Text.Length == 0 && path.Path.First().Name == Language.Start.Name);

                if (emptyStringPath == null) throw new ArgumentException("The lagnauge does not allow for an empty string input.");

                foreach (var path in emptyStringPath.Path)
                {
                    yield return new Tuple<GrammarRulePath, string>(path, Language.Start.Name.ToString());
                }

                yield break;
            }

            var derivation = new StringBuilder(Language.Start.Name.ToString());
            var readInput = new StringBuilder();
            var symbolStack = new List<char>
            {
                Language.Start.Name
            };
            var inputStream = new List<char>(input);

            while (inputStream.Count > 0)
            {
                var token = inputStream.PullFromStart();//K will be sued later
                var symbol = symbolStack.PullFromStart();

                if (symbol == token)
                {
                    //do nothing because it is an expected terminal
                    readInput.Append(token);
                }
                else
                {
                    var key = getKey(symbol, token);

                    GrammarRulePath rule;
                    if (!LlParsingTable.TryGetValue(key, out rule))
                        throw new ArgumentException("Unable to parse input starting at: " + inputStream.Count);

                    symbolStack.PushToStart(Language[rule.Name].Rules[rule.RuleIndex]);
                    inputStream.PushToStart(token);

                    yield return new Tuple<GrammarRulePath, string>(rule, derivation.ToString());

                    derivation.Clear();
                    derivation.Append(readInput);
                    derivation.Append(symbolStack.ToArray());
                }
            }
        }
        public Language Language { get; private set; }
        public bool StackOverFlowInSearch { get; set; }
        public int K { get; private set; }
        public int MaxRecursionSearch { get; set; }
    }
    public class TreePath
    {
        public TreePath(TreeRoot root)
        {
            Path = new List<GrammarRulePath>();
            Root = root;
            Text = new StringBuilder();
        }
        public TreePath(TreePath copy)
        {
            Level = copy.Level + 1;
            Path = new List<GrammarRulePath>(copy.Path);
            Root = copy.Root;
            Text = new StringBuilder(copy.Text.ToString());
        }
        public List<GrammarRulePath> Path { get; set; }
        public StringBuilder Text { get; set; }
        public TreeRoot Root { get; set; }
        public int Level { get; set; }
        public override string ToString()
        {
            return String.Format("{0} -> {1}", String.Join(", ", Path), Text);
        }
    }
    public class GrammarRulePath
    {
        public GrammarRulePath(char name, int ruleIndex)
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
