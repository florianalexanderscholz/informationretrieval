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
using InformationRetrieval.Index;

namespace InformationRetrieval.PostingListUtils
{
    /// <summary>
    /// Implements the Boolean Utils
    /// </summary>
    public static class BooleanUtils
    {
        public static SortedSet<Index.Posting> And(this SortedSet<Index.Posting> me, SortedSet<Index.Posting> other)
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
                            answer.Add(meEnumerator.Current);
                            meNext = meEnumerator.MoveNext();
                            otherNext = otherEnumerator.MoveNext();
                        }
                        else if (string.Compare(meEnumerator.Current.Document,
                                     otherEnumerator.Current.Document, StringComparison.OrdinalIgnoreCase) < 0)
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

        public static SortedSet<Index.Posting> Not(this SortedSet<Index.Posting> me, IIndex index)
        {
            if (me == null || index == null)
            {
                return new SortedSet<Index.Posting>();
            }

            var other = index.GetAllDocuments();

            var answer = new SortedSet<Index.Posting>();

            using (var meEnumerator = me.GetEnumerator())
            {
                using (var otherEnumerator = other.GetEnumerator())
                {

                    bool meNext = meEnumerator.MoveNext();
                    bool otherNext = otherEnumerator.MoveNext();

                    while (meNext == true && otherNext == true)
                    {
                        if (string.CompareOrdinal(meEnumerator.Current.Document, otherEnumerator.Current.Document) == 0)
                        {
                            meNext = meEnumerator.MoveNext();
                            otherNext = otherEnumerator.MoveNext();
                        }
                        else if (string.Compare(otherEnumerator.Current.Document,
                                     meEnumerator.Current.Document, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            answer.Add(otherEnumerator.Current);
                            otherNext = otherEnumerator.MoveNext();
                        }
                    }

                    if (otherNext == true)
                    {
                        do
                        {
                            answer.Add(otherEnumerator.Current);
                        } while (otherEnumerator.MoveNext());
                    }
                }
            }
            return answer;
        }

        public static SortedSet<Index.Posting> Or(this SortedSet<Index.Posting> me, SortedSet<Index.Posting> other)
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
                            answer.Add(meEnumerator.Current);
                            meNext = meEnumerator.MoveNext();
                            otherNext = otherEnumerator.MoveNext();
                        }
                        else if (string.Compare(meEnumerator.Current.Document, otherEnumerator.Current.Document,
                                     StringComparison.OrdinalIgnoreCase) < 0)
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

                    if (meNext == true)
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
                }
            }

            return answer;
        }

        public static SortedSet<Index.Posting> ProximityAndSymmetric(this SortedSet<Index.Posting> me,
            SortedSet<Index.Posting> other, int k)
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
                        if (string.Compare(meEnumerator.Current.Document, otherEnumerator.Current.Document,
                                StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var l = new List<int>();
                            var myPositions = meEnumerator.Current.Positions;
                            var otherPositions = otherEnumerator.Current.Positions;

                            using (var myPosEnumerator = myPositions.GetEnumerator())
                            {
                                using (var otherPosEnumerator = otherPositions.GetEnumerator())
                                {
                                    bool mePosNext = myPosEnumerator.MoveNext();
                                    bool otherPosNext = otherPosEnumerator.MoveNext();

                                    while (mePosNext == true)
                                    {
                                        while (otherPosNext == true)
                                        {
                                            if (Math.Abs(myPosEnumerator.Current - otherPosEnumerator.Current) <= k)
                                            {
                                                l.Add(otherPosEnumerator.Current);
                                            }
                                            else if (otherPosEnumerator.Current > myPosEnumerator.Current)
                                            {
                                                break;
                                            }

                                            otherPosNext = otherPosEnumerator.MoveNext();
                                        }

                                        while (l.Any() && Math.Abs(l.First() - myPosEnumerator.Current) > k)
                                        {
                                            l.RemoveAt(0);
                                        }

                                        foreach (var element in l)
                                        {
                                            answer.Add(new Posting(meEnumerator.Current.Document)
                                            {
                                                Positions = new SortedSet<int>()
                                                {
                                                    myPosEnumerator.Current,
                                                    element
                                                }
                                            });
                                        }

                                        mePosNext = myPosEnumerator.MoveNext();
                                        //l.RemoveWhere(m => (m - myPosEnumerator.Current) > k);
                                    }

                                }
                            }

                            meNext = meEnumerator.MoveNext();
                            otherNext = otherEnumerator.MoveNext();

                        }
                        else
                        {
                            if (string.Compare(meEnumerator.Current.Document, otherEnumerator.Current.Document,
                                    StringComparison.OrdinalIgnoreCase) < 0)
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
        }

        public static SortedSet<Index.Posting> ProximityAndAsymmetric(this SortedSet<Index.Posting> me,
                SortedSet<Index.Posting> other, int k)
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
                            if (string.Compare(meEnumerator.Current.Document, otherEnumerator.Current.Document, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                var l = new List<int>();
                                var myPositions = meEnumerator.Current.Positions;
                                var otherPositions = otherEnumerator.Current.Positions;

                                using (var myPosEnumerator = myPositions.GetEnumerator())
                                {
                                    using (var otherPosEnumerator = otherPositions.GetEnumerator())
                                    {
                                        bool mePosNext = myPosEnumerator.MoveNext();
                                        bool otherPosNext = otherPosEnumerator.MoveNext();

                                        while (mePosNext == true)
                                        {
                                            while (otherPosNext == true)
                                            {
                                                if (Math.Abs(myPosEnumerator.Current - otherPosEnumerator.Current) <= k && myPosEnumerator.Current < otherPosEnumerator.Current)
                                                {
                                                    l.Add(otherPosEnumerator.Current);
                                                }
                                                else if (otherPosEnumerator.Current > myPosEnumerator.Current)
                                                {
                                                    break;
                                                }

                                                otherPosNext = otherPosEnumerator.MoveNext();
                                            }

                                            while (l.Any() && Math.Abs(l.First() - myPosEnumerator.Current) > k)
                                            {
                                                l.RemoveAt(0);
                                            }

                                            foreach (var element in l)
                                            {
                                                answer.Add(new Posting(meEnumerator.Current.Document)
                                                {
                                                    Positions = new SortedSet<int>()
                                                    {
                                                        myPosEnumerator.Current,
                                                        element
                                                    }
                                                });
                                            }

                                            mePosNext = myPosEnumerator.MoveNext();
                                            //l.RemoveWhere(m => (m - myPosEnumerator.Current) > k);
                                        }

                                    }
                                }

                                meNext = meEnumerator.MoveNext();
                                otherNext = otherEnumerator.MoveNext();

                            }
                            else
                            {
                                if (string.Compare(meEnumerator.Current.Document, otherEnumerator.Current.Document, StringComparison.OrdinalIgnoreCase) < 0)
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
            }
    }
}
