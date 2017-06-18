using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearRegression;

namespace InformationRetrieval.Index
{
    public class Cluster
    {
        public Cluster(Document doc)
        {
            this.Leader = doc;
        }

        public Document Leader { get; set; }

        public List<Document> Followers { get; set; } = new List<Document>();

        public double GetSimiliarity(Document otherDoc)
        {
            var set = Leader.Terms.Intersect(otherDoc.Terms);
            double score = 0.0;
            //Console.WriteLine("{0}", set.Count());
            foreach (var term in set)
            {
                var postingMe = term.Postings[Leader.DocId];
                var postingOther = term.Postings[otherDoc.DocId];
                //Console.WriteLine("{0} {1}", postingMe.Score, postingOther.Score);
                score += postingMe.Weight * postingOther.Weight;
            }

            score /= (double)Leader.Length;
            score /= (double)otherDoc.Length;
            return Math.Abs(score);
        }
    }
}