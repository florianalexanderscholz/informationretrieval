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

        public List<Posting> EvaluateBooleanExpression(string expression, IIndex indexStorage)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return new List<Posting>();
            }

            if (indexStorage == null)
            {
                return new List<Posting>();
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

            return mergedPostings.ToList();
        }

        public List<Posting> EvaluateFullPhraseQuery(string request, IIndex index)
        {
            var tokens = request.Split(new char[] {' '}).ToArray();
            int degree = tokens.Count();

            if (degree >= 1)
            {
                return performFullPhraseQuery(index, tokens);
            }

            return new List<Posting>();
        }

        private List<Posting> performFullPhraseQuery(IIndex index, string[] tokens)
        {
            var words = (from t in tokens select t.ToLower()).ToArray();

            var sortedSet = new List<Term>();

            bool allMatches = true;
            for (int i = 0; i < words.Length; i++)
            {
                allMatches &= index.GetPosting(words[i], out Term exportedTerm);
                sortedSet.Add(exportedTerm);
            }

            if (allMatches == false)
            {
                return new List<Posting>();
            }

            using (var enumerator = sortedSet.GetEnumerator())
            {
                enumerator.MoveNext();

                var matchedPostings = enumerator.Current.Postings;
                while (enumerator.MoveNext())
                {
                    matchedPostings = matchedPostings.ProximityAndAsymmetric(enumerator.Current.Postings, 1);

                }
                return matchedPostings.ToList();
            }
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