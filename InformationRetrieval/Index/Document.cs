using System;
using System.Collections.Generic;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    public class Document
    {
        public string Filename { get; set; } = "";
        public int Length { get; set; }
        public List<Term> Terms { get; set; } = new List<Term>();
        public Dictionary<int, double> Correlation { get; set; } = new Dictionary<int, double>();
        public int[] DocVec;
        public int DocId { get; set; }
        public Document(int docId)
        {
            DocId = docId;
        }
    }
}