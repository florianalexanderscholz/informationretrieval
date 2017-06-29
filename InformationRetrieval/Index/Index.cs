using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InformationRetrieval.PostingListUtils;
using InformationRetrieval.QueryProcessor;
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
        public int KGram { get; set; } = 5;
        public KGramList kgramList = new KGramList();
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

                term.Name = token.Value;
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
                    Terms.Add(token.Value.ToLower(), term);
                }

                kgramList.Add(term, token.Value, KGram);
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
            List<string> words = query.ToLower().Split(' ').ToList();
            List<string> correctedWords = new List<string>();

            if (words.Count == 0)
            {
                return new List<Hit>();
            }

            foreach (var word in words)
            {
                bool found = GetPosting(word, out Term foundTerm);
                if (found == false || foundTerm.Postings.Count < r)
                {
                    Console.WriteLine("Suppose the term {0} is miswritten", word);
                    Console.WriteLine("- Candidates are: ");
                    
                    var candidates = kgramList.GetWords(word, KGram);
                    foreach (var cand in candidates)
                    {
                        Console.WriteLine(cand);
                    }
                    correctedWords.AddRange(candidates);
                }
                else
                {
                    correctedWords.Add(word);
                }
            }

            double[] score = new double[AllDocuments.Count];
            string[] docs = new string[AllDocuments.Count];
            foreach (var document in AllDocuments)
            {
                docs[document.Value.DocId] = document.Value.Filename;
                score[document.Value.DocId] = 0.0;
                foreach (var term in correctedWords)
                {
                    bool found = GetPosting(term, out Term foundTerm);
                    if (found == false || foundTerm.Postings.Count < r)
                    {
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
                //Console.WriteLine("{0} ({1})", docs[i], score[i]);
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

            double[] score = new double[AllDocuments.Count];
            string[] docs = new string[AllDocuments.Count];

            Cluster maxCluster = Clusters.First();
            double maxScore = 0;
            SortedList<double, Cluster> clusterSet = new SortedList<double, Cluster>(new ReverseDuplicateKeyComparer<double>());
            foreach (var cluster in Clusters)
            {
                var cdocument = cluster.Leader;

                docs[cdocument.DocId] = cdocument.Filename;
                score[cdocument.DocId] = 0.0;
                double docScore = 0;
                foreach (var term in words)
                {
                    bool found = GetPosting(term, out Term foundTerm);
                    if (found == false)
                    {
                        //Console.WriteLine("Term not found: {0}", term);
                        continue;
                    }
                    if (foundTerm.Postings.TryGetValue(cdocument.DocId, out Posting posting))
                    {
                        score[cdocument.DocId] += posting.Weight;
                    }
                    else
                    {
                        score[cdocument.DocId] += 0;
                    }
                    score[cdocument.DocId] /= cdocument.Length;

                   
                }
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
                score[document.DocId] = 0.0;
                foreach (var term in words)
                {
                    bool found = GetPosting(term, out Term foundTerm);
                    if (found == false)
                    {
                        Console.WriteLine("Term not found: {0}", term);
                        continue;
                    }
                    if (foundTerm.Postings.TryGetValue(document.DocId, out Posting posting))
                    {
                        score[document.DocId] += posting.Weight;
                    }
                    else
                    {
                        score[document.DocId] += 0;
                    }
                }
                score[document.DocId] /= document.Length;
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
                termPair.Value.KgramSet = termPair.Key.KGrams(KGram);
            }
            kgramList.Finish();

            

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
                var sortedList = new SortedList<double, Cluster>(new ReverseDuplicateKeyComparer<double>());
                double score = 0;

                foreach (var cluster in Clusters)
                {
                    double sim = cluster.GetSimiliarity(doc.Value);
                    sortedList.Add(sim, cluster);
                }

                var matches = sortedList.Take(3);
                foreach (var cluster in matches)
                {
                    //Console.WriteLine("{0}", cluster.Key);
                    cluster.Value.Followers.Add(doc.Value);


                    //Console.WriteLine("Add {0} to cluster {1}, scored: {2}", doc.Value.Filename, cluster.Value.Leader.Filename, cluster.Key);
                }
            }

            sw.Stop();
            Console.WriteLine("Done: {0}s", sw.Elapsed.TotalSeconds);
        }
    }
}