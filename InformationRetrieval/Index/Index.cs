using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InformationRetrieval.QueryProcessor;
using InformationRetrieval.Tokenizer;
using InformationRetrieval.TypoCorrection;

namespace InformationRetrieval.Index
{
    public class ReverseComparer: IComparer<double>
    {
        public int Compare(double x, double y)
        {
            // Compare y and x in reverse order.
            return y.CompareTo(x);
        }
    }
    
    public class Index : IIndex
    {
        public int KGram { get; set; } = 5;
        public KGramList kgramList = new KGramList();
        public TermList termList = new TermList();
        public DocumentList documentList = new DocumentList();


        public List<Cluster> Clusters = new List<Cluster>();
        public void InsertPostings(List<Token> tokens, string filename, int docId)
        {
            if (tokens == null || string.IsNullOrEmpty(filename))
            {
                return;
            }

            documentList.AddDocument(filename, docId, tokens);

            int i = 0;
            foreach (var token in tokens)
            {
              
                var term = termList.AddTerm(token, i, docId, filename);
                kgramList.AddTermToTypoCorrection(term, KGram);
                i++;
            }


        }

        public List<Hit> PerformSearch(string query, IQueryFlags queryFlags, int r)
        {
            List<Hit> matches;

            if (queryFlags is VectorSearchFlags)
            {
                matches = PerformSearch_Simple(query, queryFlags, false, r);
            }
            else if (queryFlags is ClusterSearchFlags)
            {
                matches = PerformSearch_Clustering(query, queryFlags, false, r);
            }
            else
            {
                matches = new List<Hit>();
            }

            return matches;
        }

        private List<Hit> PerformSearch_Simple(string query, IQueryFlags queryFlags, bool useLeveshtein, int r)
        {
            var vectorSearchFlags = queryFlags as VectorSearchFlags;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var searchTerms = termList.GetTermsByQuery(query, kgramList, KGram, r);

            if (searchTerms.Count == 0)
            {
                return new List<Hit>();
            }

            double[] score = new double[documentList.DocumentCount];
            string[] docs = new string[documentList.DocumentCount];
            foreach (var document in documentList.Documents)
            {
                docs[document.DocId] = document.Filename;
                score[document.DocId] = documentList.CalculateScore(termList, document, searchTerms, r);
            }
            sw.Stop();
            Console.WriteLine("Vector Search: {0}", sw.Elapsed.TotalSeconds);

            Array.Sort(score, docs, new ReverseComparer());
            List<Hit> hits = new List<Hit>();
            for (int i = 0; i < vectorSearchFlags.K && score[i] > 0.0001; i++)
            {
                hits.Add(new Hit()
                {
                    Document = docs[i],
                    Score = score[i]
                });
            }

            return hits;
        }

        private List<Hit> PerformSearch_Clustering(string query, IQueryFlags queryFlags, bool useLevenshtein, int r)
        {
            var clusterSearchFlags = queryFlags as ClusterSearchFlags;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> words = query.ToLower().Split(' ').ToList();
            if (words.Count == 0)
            {
                return new List<Hit>();
            }

            double[] score = new double[documentList.DocumentCount];
            string[] docs = new string[documentList.DocumentCount];

            SortedList<double, Cluster> clusterSet = new SortedList<double, Cluster>(new ReverseDuplicateKeyComparer<double>());
            foreach (var cluster in Clusters)
            {
                var cdocument = cluster.Leader;

                docs[cdocument.DocId] = cdocument.Filename;
                score[cdocument.DocId] = documentList.CalculateScore(termList, cdocument, words, r);
               
                clusterSet.Add(score[cdocument.DocId], cluster);
            }

            var clusterSubset = clusterSet.Take(clusterSearchFlags.B1);
            List<Document> relevantDocuments = new List<Document>();
            foreach (var c in clusterSubset)
            {
                foreach (var d in c.Value.Followers)
                {
                    relevantDocuments.Add(d);
                }
            }

            score = new double[score.Length];
            foreach (var document in relevantDocuments)
            {
                docs[document.DocId] = document.Filename;
                score[document.DocId] = documentList.CalculateScore(termList, document, words, r);
            }
            sw.Stop();
            Console.WriteLine("Cluster Pruning: {0}", sw.Elapsed.TotalSeconds);

            Array.Sort(score, docs, new ReverseComparer());

            List<Hit> hits = new List<Hit>();
            for (int i = 0; i < clusterSearchFlags.B2  && score[i] > 0.0001; i++)
            {
                hits.Add(new Hit()
                {
                    Document = docs[i],
                    Score = score[i]
                });
                //Console.WriteLine("{0} ({1})", docs[i], score[i]);
            }

            return hits;
        }


        public void Finish()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            termList.Prepare(documentList, KGram);
            kgramList.Prepare();

            termList.CalculateScores(documentList);

            var nCluster = Math.Sqrt(documentList.DocumentCount);
            var allDocs = documentList.Documents.ToArray();
            Random shuffler = new Random();
            shuffler.Shuffle(allDocs);

            for (int i = 0; i < nCluster; i++)
            {
                Clusters.Add(new Cluster(allDocs[i]));
            }

            foreach (var doc in documentList.Documents.ToArray())
            {
                var sortedList = new SortedList<double, Cluster>(new ReverseDuplicateKeyComparer<double>());
                double score = 0;

                foreach (var cluster in Clusters)
                {
                    double sim = cluster.GetSimiliarity(doc);
                    sortedList.Add(sim, cluster);
                }

                var matches = sortedList.Take(3);
                foreach (var cluster in matches)
                {
                    //Console.WriteLine("{0}", cluster.Key);
                    cluster.Value.Followers.Add(doc);


                    //Console.WriteLine("Add {0} to cluster {1}, scored: {2}", doc.Value.Filename, cluster.Value.Leader.Filename, cluster.Key);
                }
            }

            sw.Stop();
            Console.WriteLine("Done: {0}s", sw.Elapsed.TotalSeconds);
        }
    }
}