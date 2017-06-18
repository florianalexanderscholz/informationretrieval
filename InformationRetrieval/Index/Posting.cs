﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InformationRetrieval.Index
{
    public class Posting : IComparable
    {
        public Posting(string document, int docId)
        {
            this.Document = document;
            this.DocId = docId;
        }

        public Posting(string document)
        {
            this.Document = document;
        }

        public int DocId { get; set; }
        
        public SortedSet<int> Positions { get; set; } = new SortedSet<int>();

        public string Document { get; set; }

        public int TermFrequency { get; set; }

        public double Weight { get; set; }

        public int CompareTo(object obj)
        {
            Posting b = (Posting) obj;
            return String.Compare(Document, b.Document, StringComparison.OrdinalIgnoreCase);
        }

        public double Score { get; set; } = 0.0;
    }
}
