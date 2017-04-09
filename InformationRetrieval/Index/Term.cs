using System.Collections.Generic;
using System.Reflection.Metadata;

namespace InformationRetrieval.Index
{
    public class Term
    {
        public SortedSet<Posting> Postings { get; set; } = new SortedSet<Posting>();
    }
}