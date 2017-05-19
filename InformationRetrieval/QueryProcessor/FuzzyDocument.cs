using System.Collections.Generic;
using InformationRetrieval.ExpressionParser;

namespace InformationRetrieval.QueryProcessor
{
    public class FuzzyDocument
    {
        public string Filename { get; set; } = "";

        public Dictionary<string, double> Correlation { get; set; } = new Dictionary<string, double>();
    }
}