using System.Collections.Generic;

namespace InformationRetrieval.Index
{
    public class Term
    {
        public SortedSet<Posting> Postings { get; set; } = new SortedSet<Posting>();
        public int Index { get; set; } = -1;
    }
}