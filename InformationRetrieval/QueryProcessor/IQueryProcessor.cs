using System.Collections;
using System.Collections.Generic;
using InformationRetrieval.Index;

namespace InformationRetrieval.QueryProcessor
{
    public interface IQueryProcessor
    {
        List<Posting> EvaluateBooleanExpression(string expression, IIndex indexStorage);
        List<Posting> EvaluateFullPhraseQuery(string request, IIndex index);
    }
}