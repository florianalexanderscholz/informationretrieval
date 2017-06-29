using System;
using System.Collections.Generic;
using System.Linq;
using InformationRetrieval.Index;

namespace InformationRetrieval.Utils
{
    public static class BooleanUtils
    {
        public static SortedSet<Term> And(this SortedSet<Term> me, SortedSet<Term> other)
        {
            if (me == null || other == null)
            {
                return new SortedSet<Term>();
            }

            var answer = new SortedSet<Term>();

            using (var meEnumerator = me.GetEnumerator())
            {
                using (var otherEnumerator = other.GetEnumerator())
                {

                    bool meNext = meEnumerator.MoveNext();
                    bool otherNext = otherEnumerator.MoveNext();

                    while (meNext == true && otherNext == true)
                    {
                        if (meEnumerator.Current.CompareTo(otherEnumerator.Current) == 0)
                        {
                            answer.Add(meEnumerator.Current);
                            meNext = meEnumerator.MoveNext();
                            otherNext = otherEnumerator.MoveNext();
                        }
                        else if (meEnumerator.Current.CompareTo(otherEnumerator.Current) < 0)
                        {
                            meNext = meEnumerator.MoveNext();
                        }
                        else
                        {
                            otherNext = otherEnumerator.MoveNext();
                        }
                    }
                }
            }
            return answer;
        }
        
        public static int AndCount(this SortedSet<string> me, SortedSet<string> other)
        {
            if (me == null || other == null)
            {
                return 0;
            }

            int answerCount = 0; ;

            using (var meEnumerator = me.GetEnumerator())
            {
                using (var otherEnumerator = other.GetEnumerator())
                {

                    bool meNext = meEnumerator.MoveNext();
                    bool otherNext = otherEnumerator.MoveNext();

                    while (meNext == true && otherNext == true)
                    {
                        if (meEnumerator.Current.CompareTo(otherEnumerator.Current) == 0)
                        {
                            answerCount++;
                            meNext = meEnumerator.MoveNext();
                            otherNext = otherEnumerator.MoveNext();
                        }
                        else if (string.Compare(meEnumerator.Current,
                                     otherEnumerator.Current, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            meNext = meEnumerator.MoveNext();
                        }
                        else
                        {
                            otherNext = otherEnumerator.MoveNext();
                        }
                    }
                }
            }

            return answerCount;
        }

        public static SortedSet<Index.Posting> AndNot(this SortedSet<Index.Posting> me, SortedSet<Index.Posting> other)
        {
            if (me == null || other == null)
            {
                return new SortedSet<Index.Posting>();
            }

            var answer = new SortedSet<Index.Posting>();

            using (var meEnumerator = me.GetEnumerator())
            {
                using (var otherEnumerator = other.GetEnumerator())
                {

                    bool meNext = meEnumerator.MoveNext();
                    bool otherNext = otherEnumerator.MoveNext();

                    while (meNext == true && otherNext == true)
                    {
                        if (meEnumerator.Current.CompareTo(otherEnumerator.Current) == 0)
                        {
                            meNext = meEnumerator.MoveNext();
                            otherNext = otherEnumerator.MoveNext();
                        }
                        else if (string.Compare(meEnumerator.Current.Document,
                                     otherEnumerator.Current.Document, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            answer.Add(meEnumerator.Current);
                            meNext = meEnumerator.MoveNext();
                        }
                        else
                        {
                            otherNext = otherEnumerator.MoveNext();
                        }
                    }

                    if (meNext == true)
                    {
                        do
                        {
                            answer.Add(meEnumerator.Current);
                        } while (meEnumerator.MoveNext());
                    }
                }
            }
            return answer;
        }
    }
}
