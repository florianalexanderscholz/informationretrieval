using System;
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
                SortedList<int, PostingEntry> postingList = new SortedList<int, PostingEntry>(new DuplicateKeyComparer<int>());

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

                    PostingEntry postingEntry = new PostingEntry()
                    {
                        Variable = variable,
                        PostingSet = postings
                    };

                    int priority = postings.Count;

                    if (variable.Negative)
                    {
                        priority = Int32.MaxValue;
                    }
                    postingList.Add(priority, postingEntry);
                }

                SortedSet<Posting> postingSet = new SortedSet<Posting>();

                using (var currentMergeIterator = postingList.GetEnumerator())
                {
                    if (currentMergeIterator.MoveNext() == false)
                    {
                        /* No Elements merged */
                        continue;
                    }

                    var firstEntry = currentMergeIterator.Current.Value;
                        foreach (var element in firstEntry.PostingSet)
                        {
                            postingSet.Add(element);
                        }

                    if (firstEntry.Variable.Negative)
                    {
                        var allDocs = indexStorage.GetAllDocuments();
                        postingSet = postingSet.Not(indexStorage);
                    }

                    while (currentMergeIterator.MoveNext())
                    {
                        var currentEntry = currentMergeIterator.Current.Value;

                        if (currentEntry.Variable.Negative)
                        {
                            postingSet = postingSet.AndNot(currentEntry.PostingSet);
                        }
                        else
                        {
                            postingSet = postingSet.And(currentEntry.PostingSet);
                        }
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