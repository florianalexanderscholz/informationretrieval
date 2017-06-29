using System.Collections.Generic;
using InformationRetrieval.Index;

namespace InformationRetrieval.TypoCorrection
{
    public class KGramTerm
    {
        public string KGram { get; set; }
        public Dictionary<string, Term> Words { get; set; } = new Dictionary<string, Term>();
        public int Index { get; set; }
    }
}