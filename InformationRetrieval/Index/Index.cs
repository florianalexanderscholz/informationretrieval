using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Linq.Expressions;
using InformationRetrieval.PostingListUtils;
using InformationRetrieval.Tokenizer;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Vector = MathNet.Numerics.LinearAlgebra.Single.Vector;

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
        public SortedList<string, Term> Terms { get;  } = new SortedList<string, Term>();
        public SortedList<string, Document> AllDocuments { get; set; } = new SortedList<string, Document>();
        public Dictionary<int, Term> lookupTable = new Dictionary<int, Term>();
        SparseMatrix weight;
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

            Dictionary<string, SortedSet<int>> positionSet = new Dictionary<string,SortedSet<int>>();

            foreach (var token in tokens)
            {
                bool found_term = positionSet.TryGetValue(token.Value, out SortedSet<int> valueSet);
                if (found_term)
                {
                    valueSet.Add(token.Position);
                }
                else
                {
                    positionSet[token.Value] = new SortedSet<int> {token.Position};
                }
            }
            
            foreach (var token in positionSet)
            {
                bool found_term = false;

                if (Terms.TryGetValue(token.Key, out Term term) == true)
                {
                    found_term = true;
                }
                else
                {
                    term = new Term();
                }

                term.Postings.Add(new Posting(filename, docId)
                {
                    Positions = token.Value
                });

                if (found_term == false)
                {
                    Terms.Add(token.Key, term);
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
                    score[document.Value.DocId] += weight[foundTerm.Index, document.Value.DocId];
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
                    AllDocuments[posting.Document].Terms.Add(termPair.Value);
                }
            }

            weight = new SparseMatrix(Terms.Count, AllDocuments.Count);
            SparseMatrix termFrequency = new SparseMatrix(Terms.Count, AllDocuments.Count);
            for (int i = 0; i < Terms.Count; i++)
            {
                for (int j = 0; j < AllDocuments.Count; j++)
                {
                    termFrequency[i,j] = 0;
                }
            }

            foreach (var term in Terms.Values)
            {
                var termId = term.Index;
                foreach (var posting in term.Postings)
                {
                    var docId = posting.DocId;
                    termFrequency[termId, docId] = posting.Positions.Count;
                }
            }

            var docFrequency = new DenseVector(Terms.Count);
            

            foreach (var term in Terms.Values)
            {
                docFrequency[term.Index] = term.Postings.Count;
            }
            for (int i = 0; i < Terms.Count; i++)
            {
                for (int j = 0; j < AllDocuments.Count; j++)
                {
                    if (termFrequency[i, j] > 0)
                    {
                        weight[i, j] = (1 + Math.Log10(termFrequency[i, j])) *
                                        Math.Log(AllDocuments.Count / docFrequency[i]);
                    }
                    else
                    {
                        weight[i, j] = 0;
                    }
                }
            }
            Console.WriteLine("Done");
        }
    }
}