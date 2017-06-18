using System.Collections.Generic;

namespace InformationRetrieval.Index
{
    public class Term
    {
        public Dictionary<int, Posting> Postings { get; set; } = new Dictionary<int, Posting>();
        public int Index { get; set; } = -1;
        public int DocumentFrequency { get; set; }
    }
}