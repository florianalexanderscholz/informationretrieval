using System.Collections.Generic;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    public class Document
    {
        public string Filename { get; set; } = "";

        public List<Term> Terms { get; set; } = new List<Term>();
        public Dictionary<int, double> Correlation { get; set; } = new Dictionary<int, double>();
    }
}