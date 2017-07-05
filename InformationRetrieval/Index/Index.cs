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
        public int KGram { get; set; } = 3;
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
                matches = PerformSearch_Simple(query, queryFlags, false);
            }
            else if (queryFlags is ClusterSearchFlags)
            {
                matches = PerformSearch_Clustering(query, queryFlags, false);
            }
            else
            {
                matches = new List<Hit>();
            }

            return matches;
        }

        private List<Hit> PerformSearch_Simple(string query, IQueryFlags queryFlags, bool useLeveshtein)
        {
            var vectorSearchFlags = queryFlags as VectorSearchFlags;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var searchTerms = termList.GetTermsByQuery(query, kgramList, KGram, queryFlags.R, queryFlags.J, queryFlags.L);

            if (searchTerms.Count == 0)
            {
                return new List<Hit>();
            }

            double[] score = new double[documentList.DocumentCount];
            string[] docs = new string[documentList.DocumentCount];
            foreach (var document in documentList.Documents)
            {
                docs[document.DocId] = document.Filename;
                score[document.DocId] = documentList.CalculateScore(termList, document, searchTerms, queryFlags.R);
            }
            sw.Stop();
            Console.WriteLine("Vector Search: {0}", sw.Elapsed.TotalSeconds);

            return createRanking(score, docs, vectorSearchFlags.K);
        }

        private List<Hit> PerformSearch_Clustering(string query, IQueryFlags queryFlags, bool useLevenshtein)
        {
            var clusterSearchFlags = queryFlags as ClusterSearchFlags;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var searchTerms = termList.GetTermsByQuery(query, kgramList, KGram, queryFlags.R, queryFlags.J, queryFlags.L);

            if (searchTerms.Count == 0)
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
                score[cdocument.DocId] = documentList.CalculateScore(termList, cdocument, searchTerms, queryFlags.R);
               
                clusterSet.Add(score[cdocument.DocId], cluster);
            }

            var clusterSubset = clusterSet.Take(clusterSearchFlags.B2);
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
                score[document.DocId] = documentList.CalculateScore(termList, document, searchTerms, queryFlags.R);
            }
            sw.Stop();
            Console.WriteLine("Cluster Pruning: {0}", sw.Elapsed.TotalSeconds);

            return createRanking(score, docs, clusterSearchFlags.K);
        }


        public void Finish(int B1, int B2)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            termList.Prepare(documentList, KGram);
            kgramList.Prepare();

            termList.CalculateScores(documentList);
            documentList.CalculateLenghts();

            CreateClusters(B1);

            sw.Stop();
            Console.WriteLine("Done: {0}s", sw.Elapsed.TotalSeconds);
        }

        public void CreateClusters(int B1)
        {
            var nCluster = Math.Sqrt(documentList.DocumentCount);
            var allDocs = documentList.Documents.ToArray();
            Random shuffler = new Random();
            shuffler.Shuffle(allDocs);

            Clusters.Clear();
            for (int i = 0; i < nCluster; i++)
            {
                Clusters.Add(new Cluster(allDocs[i]));
            }

            foreach (var doc in documentList.Documents.ToArray())
            {
                var sortedList = new SortedList<double, Cluster>(new ReverseDuplicateKeyComparer<double>());

                foreach (var cluster in Clusters)
                {
                    double sim = cluster.GetSimiliarity(doc);
                    sortedList.Add(sim, cluster);
                }

                var matches = sortedList.Take(B1);
                foreach (var cluster in matches)
                {
                    cluster.Value.Followers.Add(doc);
                }
            }
        }

        private static List<Hit> createRanking(double[] score, string[] docs, int k)
        {
            Array.Sort(score, docs, new ReverseComparer());
            List<Hit> hits = new List<Hit>();
            for (int i = 0; i < k && score[i] > 0.0001; i++)
            {
                hits.Add(new Hit()
                {
                    Document = docs[i],
                    Score = score[i]
                });
            }

            return hits;
        }
    }
}