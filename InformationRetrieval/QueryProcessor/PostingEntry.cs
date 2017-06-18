using System.Collections.Generic;
using InformationRetrieval.Index;

namespace InformationRetrieval.QueryProcessor
{
    public class PostingEntry
    {
        public SortedSet<Posting> PostingSet {
            get;
            set;
        }
    }
}