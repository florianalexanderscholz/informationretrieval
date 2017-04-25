using System;
using System.Collections.Generic;
using System.Text;

namespace InformationRetrieval.Index
{
    public class Posting : IComparable
    {
        public Posting(string document)
        {
            this.Document = document;
        }

        public SortedSet<int> Positions { get; set; } = new SortedSet<int>();

        public string Document { get; set; }

        public int CompareTo(object obj)
        {
            Posting b = (Posting) obj;
            return String.Compare(Document, b.Document, StringComparison.OrdinalIgnoreCase);
        }
    }
}
