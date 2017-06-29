using System;
using System.Collections.Generic;
using InformationRetrieval.QueryProcessor;

namespace InformationRetrieval.Index
{
    public class Term : IComparable
    {
        public Dictionary<int, Posting> Postings { get; set; } = new Dictionary<int, Posting>();
        public int Index { get; set; } = -1;
        public int DocumentFrequency { get; set; }
        public SortedSet<string> KgramSet { get; set; } = new SortedSet<string>(new DuplicateKeyComparer<string>());
        public string Name { get; set; }
        public int CompareTo(object obj)
        {
            Term other = (Term) obj;

            return Name.CompareTo(other.Name);
        }
    }
}