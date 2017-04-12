using System.Collections.Generic;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;

namespace InformationRetrieval.QueryProcessor
{
    public class PostingEntry
    {
        public Variable Variable
        {
            get;
            set;
        }

        public SortedSet<Posting> PostingSet {
            get;
            set;
        }
    }
}