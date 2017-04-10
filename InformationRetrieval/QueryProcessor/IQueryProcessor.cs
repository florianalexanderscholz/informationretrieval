using System.Collections.Generic;
using InformationRetrieval.Index;

namespace InformationRetrieval.QueryProcessor
{
    public interface IQueryProcessor
    {
        SortedSet<Posting> EvaluateExpression(string expression, IIndex indexStorage);
    }
}