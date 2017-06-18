using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InformationRetrieval.Tokenizer;

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
        public Dictionary<string, Term> Terms { get;  } = new Dictionary<string, Term>();
        public Dictionary<string, Document> AllDocuments { get; set; } = new Dictionary<string, Document>();
        public Dictionary<int, Term> lookupTable = new Dictionary<int, Term>();
        public List<Cluster> Clusters = new List<Cluster>();
        public void InsertPostings(List<Token> tokens, string filename, int docId)
        {
            if (tokens == null || string.IsNullOrEmpty(filename))
            {
                return;
            }

            AllDocuments.Add(filename, new Document(docId)
            {
                Filename = filename,
                Length = tokens.Count
                
            });
            int i = 0;
            foreach (var token in tokens)
            {
                
                bool found_term = false;

                if (Terms.TryGetValue(token.Value, out Term term) == true)
                {
                    found_term = true;
                }
                else
                {
                    term = new Term();
                }

                if (term.Postings.ContainsKey(docId) == false)
                {
                    term.Postings.Add(docId, new Posting(filename, docId)
                    {
                        Positions = new SortedSet<int>()
                        {
                            i
                        }
                    });
                }
                else
                {
                    term.Postings[docId].Positions.Add(i);
                }

                if (found_term == false)
                {
                    Terms.Add(token.Value, term);
                }
            }


        }

        public SortedSet<Posting> GetAllDocuments()
        {
            var postingList = new SortedSet<Posting>();

            foreach (var doc in AllDocuments)
            {
                postingList.Add(new Posting(doc.Key));
            }

            return postingList;
        }

        public bool GetPosting(string token, out Term term)
        {
            if (string.IsNullOrEmpty(token))
            {
                term = null;
                return false;
            }

            if (Terms.TryGetValue(token, out term) == false)
            {
                return false;
            }

            return true;
        }

        public void PerformSearch(string query)
        {
            List<string> words = query.ToLower().Split(' ').ToList();
            double[] score = new double[AllDocuments.Count];
            string[] docs = new string[AllDocuments.Count];
            foreach (var document in AllDocuments)
            {
                docs[document.Value.DocId] = document.Value.Filename;
                score[document.Value.DocId] = 0.0;
                foreach (var term in words)
                {
                    bool found = GetPosting(term, out Term foundTerm);
                    if (found == false)
                    {
                        Console.WriteLine("Term not found: {0}", term);
                        continue;
                    }
                    if (foundTerm.Postings.TryGetValue(document.Value.DocId, out Posting posting))
                    {
                        score[document.Value.DocId] += posting.Weight;
                    }
                    else
                    {
                        score[document.Value.DocId] += 0;
                    }
                }
                score[document.Value.DocId] /= document.Value.Length;
            }

            Array.Sort(score, docs, new ReverseComparer());

            for (int i = 0; i < 5 && score[i] > 0.0001; i++)
            {
                Console.WriteLine("{0} ({1})", docs[i], score[i]);
            }

    }


        public void Finish()
        {
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var termPair in Terms)
            {
                termPair.Value.Index = counter;
                lookupTable.Add(counter++, termPair.Value);
                foreach (var posting in termPair.Value.Postings)
                {
                    AllDocuments[posting.Value.Document].Terms.Add(termPair.Value);
                    posting.Value.TermFrequency = posting.Value.Positions.Count;
                }

                termPair.Value.DocumentFrequency = termPair.Value.Postings.Count;
            }

            foreach (var doc in AllDocuments)
            {
                /*
                var currentDoc = doc.Value;

                var length = 0.0;

                foreach (var term in currentDoc.Terms)
                {
                    length += Math.Pow(term.Postings[doc.Value.DocId].TermFrequency, 2.0);
                }

                doc.Value.Length = Math.Sqrt(length);
                */
            }

            foreach (var termPair in Terms)
            {
                foreach (var posting in termPair.Value.Postings)
                {
                    if (posting.Value.TermFrequency > 0)
                    {
                        var value = (1 + Math.Log10(posting.Value.TermFrequency)) *
                                    Math.Log(AllDocuments.Count / (double) termPair.Value.DocumentFrequency);
                        posting.Value.Weight = value;
                    }
                    else
                    {
                        posting.Value.Weight = 0;
                    }
                }

            }

            var nCluster = Math.Sqrt(AllDocuments.Count);
            var allDocs = AllDocuments.Values.ToArray();
            Random shuffler = new Random();
            shuffler.Shuffle(allDocs);

            for (int i = 0; i < nCluster; i++)
            {
                Clusters.Add(new Cluster(allDocs[i]));
            }

            foreach (var doc in AllDocuments)
            {
                Cluster minCluster = null;
                double score = 0;

                minCluster = Clusters.First();
                foreach (var cluster in Clusters)
                {
                    double sim = cluster.GetSimiliarity(doc.Value);
                    if (sim > score)
                    {
                        score = sim;
                        minCluster = cluster;
                    }
                }

                minCluster.Followers.Add(doc.Value);
                //Console.WriteLine("Add {0} to cluster {1}, scored: {2}", doc.Value.Filename, minCluster.Leader.Filename, score);
            }

            sw.Stop();
            Console.WriteLine("Done: {0}s", sw.Elapsed.TotalSeconds);
        }
    }
}