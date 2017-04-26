using System;
using System.Collections.Generic;
using System.Linq;
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

        public SortedSet<Posting> EvaluateBooleanExpression(string expression, IIndex indexStorage)
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

                    if (variable.Negative || variable.PositionalRestriction != 0)
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
                        postingSet = postingSet.Not(indexStorage);
                    }

                    while (currentMergeIterator.MoveNext())
                    {
                        var currentEntry = currentMergeIterator.Current.Value;

                        if (currentEntry.Variable.Negative)
                        {
                            postingSet = postingSet.AndNot(currentEntry.PostingSet);
                        }
                        else if(currentEntry.Variable.PositionalRestriction == 0)
                        {
                            postingSet = postingSet.And(currentEntry.PostingSet);
                        }
                        else
                        {
                            postingSet = postingSet.ProximityAndSymmetric(currentEntry.PostingSet,
                                currentEntry.Variable.PositionalRestriction);
                        }
                    }
                }

                upperPostings.Add(postingSet);
            }

            var mergedPostings = OrMerge(upperPostings);

            return mergedPostings;
        }

        public SortedSet<Posting> EvaluateFullPhraseQuery(string request, IIndex index)
        {
            var tokens = request.Split(new char[] {' '}).ToArray();
            int degree = tokens.Count();
            if (degree != 2)
            {
                return new SortedSet<Posting>();
            }

            var firstWord = tokens[0];
            var secondWord = tokens[1];

            var sortedSet = new SortedSet<Posting>();

            return sortedSet;
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