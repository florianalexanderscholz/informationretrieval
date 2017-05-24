using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using InformationRetrieval.PostingListUtils;
using InformationRetrieval.Tokenizer;
using MathNet.Numerics.LinearAlgebra.Double;

namespace InformationRetrieval.Index
{
    public class Index : IIndex
    {
        public SortedList<string, Term> Terms { get;  } = new SortedList<string, Term>();
        public SortedList<string, Document> AllDocuments { get; set; } = new SortedList<string, Document>();
        public Dictionary<int, Term> lookupTable = new Dictionary<int, Term>();
        public void InsertPostings(List<Token> tokens, string filename)
        {
            if (tokens == null || string.IsNullOrEmpty(filename))
            {
                return;
            }

            AllDocuments.Add(filename, new Document()
            {
                Filename = filename,
                
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

                term.Postings.Add(new Posting(filename)
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

        public double GetFuzzyScore(string document, string term)
        {
            bool foundDoc = AllDocuments.TryGetValue(document, out Document docDto);
            if (foundDoc == false)
            {
                return 0.0;
            }

            bool foundTerm = GetPosting(term, out Term termDto);
            if (foundTerm == false)
            {
                return 0.0;
            }

            bool foundCorrelation = docDto.Correlation.TryGetValue(termDto.Index, out double score);
            if (foundCorrelation == false)
            {
                return 0.0;
            }
            else
            {
                return score;
            }
        }

        private void CalculateJaccard()
        {
            Stopwatch sw = new Stopwatch();


            sw.Start();
            var termCount = lookupTable.Count;
            SparseMatrix matrix = new SparseMatrix(termCount,termCount);

            for (int i = 0; i < termCount; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    
                    var firstTerm = lookupTable[i];
                    var secondTerm = lookupTable[j];

                    var andCount = firstTerm.Postings.AndCount(secondTerm.Postings);

                    if (andCount == 0)
                    {
                        
                    }
                    else
                    {
                        /* TODO: N + M - Intersect */

                        var orCount = (firstTerm.Postings.Count + secondTerm.Postings.Count) - andCount;
                        //var orCount = firstTerm.Postings.OrCount(secondTerm.Postings);
                        var jaccard = (double) andCount / orCount;
                        if (jaccard > 0.49)
                        {
                            matrix[i, j] = jaccard;
                        }
                    }
                }
                
            }
            sw.Stop();

            Console.WriteLine("Jaccard duration: {0}", sw.Elapsed.TotalSeconds);

            sw.Restart();
            foreach (var document in AllDocuments)
            {
                
                foreach (var posting in lookupTable)
                {
                    double sum = 0.0;
                    foreach (var term in document.Value.Terms)
                    {
                        var i = posting.Value.Index;
                        var j = term.Index;
                        if (i < j)
                        {
                            var t = i;
                            i = j;
                            j = t;
                            
                        }
                        var matrix_value = matrix[i, j];
                            sum += Math.Log(1.0 - matrix[i, j]);
                    }
                    sum = 1.0 - Math.Exp(sum);
                    var v = sum.ToString();
                    //Console.WriteLine(v);
                    if (sum < 1.1)
                    {
                        document.Value.Correlation.Add(posting.Value.Index, sum);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("Owaha duration: {0}", sw.Elapsed.TotalSeconds);

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
            sw.Stop();
            Console.WriteLine("Lookup tables duration: {0}", sw.Elapsed.TotalSeconds);
            this.CalculateJaccard();
        }
    }
}