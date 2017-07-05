using System;
using System.Collections.Generic;
using InformationRetrieval.QueryProcessor;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    public class Document
    {
        public string Filename { get; set; } = "";

        public double Length { get; set; }


        public SortedSet<Term> Terms { get; set; } = new SortedSet<Term>(new DuplicateKeyComparer<Term>());
        //public Dictionary<int, double> Correlation { get; set; } = new Dictionary<int, double>();
        //public int[] DocVec;
        public int DocId { get; set; }
        public Document(int docId)
        {
            DocId = docId;
        }
    }
}