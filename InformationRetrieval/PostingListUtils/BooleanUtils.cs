using System;
using System.Collections.Generic;
using System.Text;

namespace InformationRetrieval.PostingListUtils
{
    public static class BooleanUtils
    {
        public static SortedSet<Index.Posting> And(this SortedSet<Index.Posting> me, SortedSet<Index.Posting> other)
        {
            if(me == null || other == null)
            {
                return new SortedSet<Index.Posting>();
            }
            else
            {
                var answer = new SortedSet<Index.Posting>();

                var meEnumerator = me.GetEnumerator();
                var otherEnumerator = other.GetEnumerator();

                bool meNext = meEnumerator.MoveNext();
                bool otherNext = otherEnumerator.MoveNext();

                while(meNext == true && otherNext == true)
                {
                    if(meEnumerator.Current.CompareTo(otherEnumerator.Current) == 0)
                    {
                        answer.Add(meEnumerator.Current);
                        meNext = meEnumerator.MoveNext();
                        otherNext = otherEnumerator.MoveNext();
                    }
                    else if(string.Compare(meEnumerator.Current.Document,otherEnumerator.Current.Document) < 0)
                    {
                        meNext = meEnumerator.MoveNext();
                    }
                    else
                    {
                        otherNext = otherEnumerator.MoveNext();
                    }
                }

                return answer;
            }
        }

        public static SortedSet<Index.Posting> Or(this SortedSet<Index.Posting> me, SortedSet<Index.Posting> other)
        {
            if (me == null || other == null)
            {
                return new SortedSet<Index.Posting>();
            }
            else
            {
                var answer = new SortedSet<Index.Posting>();

                var meEnumerator = me.GetEnumerator();
                var otherEnumerator = other.GetEnumerator();

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
                    else if (string.Compare(meEnumerator.Current.Document, otherEnumerator.Current.Document) < 0)
                    {
                        answer.Add(meEnumerator.Current);
                        meNext = meEnumerator.MoveNext();
                    }
                    else
                    {
                        answer.Add(otherEnumerator.Current);
                        otherNext = otherEnumerator.MoveNext();
                    }
                }

                if(meNext == true)
                {
                    do
                    {
                        answer.Add(meEnumerator.Current);
                    } while (meEnumerator.MoveNext());
                }

                if (otherNext == true)
                {
                    do
                    {
                        answer.Add(otherEnumerator.Current);
                    } while (otherEnumerator.MoveNext());
                }

                return answer;
            }
        }
    }
}
