using System.Collections;
using System.Collections.Generic;
using InformationRetrieval.Index;

namespace InformationRetrieval.QueryProcessor
{
    public interface IQueryProcessor
    {
        SortedSet<Posting> EvaluateBooleanExpression(string expression, IIndex indexStorage);
        SortedSet<Posting> EvaluateFullPhraseQuery(string request, IIndex index);
    }
}