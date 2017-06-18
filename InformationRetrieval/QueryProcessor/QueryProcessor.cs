using System;
using System.Collections.Generic;
using System.Linq;
using InformationRetrieval.Index;
using InformationRetrieval.PostingListUtils;

namespace InformationRetrieval.QueryProcessor
{
    public class QueryProcessor : IQueryProcessor
    {
        public QueryProcessor()
        {
        }

        public List<Posting> EvaluateBooleanExpression(string expression, IIndex indexStorage)
        {
            return new List<Posting>();
        }

        public List<Posting> EvaluateFullPhraseQuery(string request, IIndex index)
        {
            return new List<Posting>();
        }
    }
}