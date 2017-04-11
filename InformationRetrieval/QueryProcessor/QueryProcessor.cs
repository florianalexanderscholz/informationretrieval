using System.Collections.Generic;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;
using InformationRetrieval.PostingListUtils;

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

                foreach (var variable in conjunction.Variables)
                {
                    bool receivedValues = indexStorage.GetPosting(variable.Value, out Term term);
                    SortedSet<Posting> postings = new SortedSet<Posting>();

                    if (receivedValues)
                    {
                        foreach (var posting in term.Postings)
                        {
                            postings.Add(posting);
                        }
                    }

                    postingList.Add(variable.Value, postings);
                }

                SortedSet<Posting> postingSet = new SortedSet<Posting>();

                using (var currentMergeIterator = postingList.GetEnumerator())
                {
                    if (currentMergeIterator.MoveNext() == false)
                    {
                        /* No Elements merged */
                        continue;
                    }

                    foreach (var element in currentMergeIterator.Current.Value)
                    {
                        postingSet.Add(element);
                    }

                    while (currentMergeIterator.MoveNext())
                    {
                        postingSet = postingSet.And(currentMergeIterator.Current.Value);
                    }
                }
                upperPostings.Add(postingSet);
            }

            return OrMerge(upperPostings);
        }

        private SortedSet<Posting> OrMerge(List<SortedSet<Posting>> upperPostings)
        {
            SortedSet<Posting> upperPostingSet = new SortedSet<Posting>();
            using (var upperMergeIterator = upperPostings.GetEnumerator())
            {
                if (upperMergeIterator.MoveNext() == false)
                {
                    return upperPostingSet;
                }

                foreach (var element in upperMergeIterator.Current)
                {
                    upperPostingSet.Add(element);
                }

                while (upperMergeIterator.MoveNext())
                {
                    upperPostingSet = upperPostingSet.Or(upperMergeIterator.Current);
                }
            }
            return upperPostingSet;
        }
    }
}