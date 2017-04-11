using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;

namespace InformationRetrieval.QueryProcessor
{
    public class QueryProcessor : IQueryProcessor
    {
        private IExpressionParser expressionParser;

        public QueryProcessor(IExpressionParser expressionParser)
        {
            this.expressionParser = expressionParser;
        }

        public SortedSet<Posting> EvaluateExpression(string expression, IIndex indexStorage)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return new SortedSet<Posting>();
            }

            if (indexStorage == null)
            {
                return new SortedSet<Posting>();
            }

            var dnfTree = expressionParser.ParseExpression(expression);

            List<SortedSet<Posting>> upperPostings = new List<SortedSet<Posting>>();

            foreach (var conjunction in dnfTree.Conjunctions)
            {
                Dictionary<string, SortedSet<Posting>> postingList = new Dictionary<string, SortedSet<Posting>>();

                foreach(var variable in conjunction.Variables)
                {
                    bool receivedValues = indexStorage.GetPosting(variable.Value, out Term term);
                    SortedSet<Posting> postings = new SortedSet<Posting>();

                    if(receivedValues)
                    {
                        foreach (var posting in term.Postings)
                        {
                            postings.Add(posting);
                        }
                    }

                    postingList.Add(variable.Value, postings);
                }

                if(postingList.Count == 1)
                {
                    var firstPosting = postingList.Values.FirstOrDefault();

                    upperPostings.Add(firstPosting);
                }
            }

            if(upperPostings.Count == 1)
            {
                return upperPostings.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}