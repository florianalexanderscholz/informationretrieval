using System;
using System.Collections.Generic;
using System.Linq;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;
using MathNet.Numerics.Integration;

namespace InformationRetrieval.QueryProcessor
{
    public class FuzzyQueryProcessor : IQueryProcessor
    {
        private IExpressionParser expressionParser;

        public FuzzyQueryProcessor(IExpressionParser expressionParser)
        {
            this.expressionParser = expressionParser;
        }

        public List<Posting> EvaluateBooleanExpression(string expression, IIndex indexStorage)
        {
            var dnfTree = expressionParser.ParseExpression(expression);

            if (string.IsNullOrEmpty(expression))
            {
                return new List<Posting>();
            }

            if (indexStorage == null)
            {
                return new List<Posting>();
            }

            SortedSet<Posting> postingSet = new SortedSet<Posting>();

            foreach (var doc in indexStorage.GetAllDocuments())
            {
                double globalScore = 0.0;
                foreach (var conjunction in dnfTree.Conjunctions)
                {
                    double calculatedScore = 1.0;
                    foreach (var variable in conjunction.Variables)
                    {
                        var documentValue = indexStorage.GetFuzzyScore(doc.Document, variable.Value);
                        if (variable.Negative == false)
                        {
                            calculatedScore = Math.Min(calculatedScore, documentValue);
                        }
                        else
                        {
                            calculatedScore = Math.Min(calculatedScore, 1 - documentValue);
                        }
                    }
                    globalScore = Math.Max(globalScore, calculatedScore);
                }
                postingSet.Add(new Posting(doc.Document)
                {
                    Score = globalScore
                });
            }


            var sortedList = postingSet.OrderByDescending(m => m.Score).Take(10).ToList();
            return sortedList;
        }

        public List<Posting> EvaluateFullPhraseQuery(string request, IIndex index)
        {
            Console.WriteLine("Fullphrase Query not supported");
            return new List<Posting>();
        }
    }
}