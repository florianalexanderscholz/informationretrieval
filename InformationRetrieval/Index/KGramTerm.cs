using System.Collections.Generic;

namespace InformationRetrieval.Index
{
    public class KGramTerm
    {
        public string KGram { get; set; }
        public Dictionary<string, Term> Words { get; set; } = new Dictionary<string, Term>();
        public int Index { get; set; }
    }
}