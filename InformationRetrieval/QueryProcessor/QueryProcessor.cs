/*
(c) 2019 by Florian Scholz
This file is part of InformationRetrieval.

    InformationRetrieval is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    InformationRetrieval is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with InformationRetrieval.  If not, see <http://www.gnu.org/licenses/>.
*/

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

            if (degree >= 1)
            {
                return performFullPhraseQuery(index, tokens);
            }

            return new SortedSet<Posting>();
        }

        private SortedSet<Posting> performFullPhraseQuery(IIndex index, string[] tokens)
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
                return new SortedSet<Posting>();
            }

            using (var enumerator = sortedSet.GetEnumerator())
            {
                enumerator.MoveNext();

                var matchedPostings = enumerator.Current.Postings;
                while (enumerator.MoveNext())
                {
                    matchedPostings = matchedPostings.ProximityAndAsymmetric(enumerator.Current.Postings, 1);

                }
                return matchedPostings;
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