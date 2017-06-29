using System;
using System.Collections.Generic;
using System.Linq;
using InformationRetrieval.Tokenizer;
using InformationRetrieval.TypoCorrection;

namespace InformationRetrieval.Index
{
    public class TermList
    {
        private Dictionary<string, Term> termDictionary = new Dictionary<string, Term>();
        private Dictionary<int, Term> lookupTable = new Dictionary<int, Term>();

        public Term AddTerm(Token token, int i, int docId, string fileName)
        {
            bool found_term = false;

            if (termDictionary.TryGetValue(token.Value, out Term term) == true)
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
                term.Postings.Add(docId, new Posting(fileName, docId)
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
                termDictionary.Add(token.Value.ToLower(), term);
            }

            return term;
        }

        public bool TryGetTermByName(string token, out Term term)
        {
            if (string.IsNullOrEmpty(token))
            {
                term = null;
                return false;
            }

            if (termDictionary.TryGetValue(token, out term) == false)
            {
                return false;
            }

            return true;
        }

        public void Prepare(DocumentList documentList, int kGramSize)
        {
            int counter = 0;

            foreach (var termPair in termDictionary)
            {
                termPair.Value.Index = counter;
                lookupTable.Add(counter++, termPair.Value);

                foreach (var posting in termPair.Value.Postings)
                {
                    documentList.AddTermToDocument(posting.Value.Document, termPair.Value);
                    posting.Value.TermFrequency = posting.Value.Positions.Count;
                }

                termPair.Value.DocumentFrequency = termPair.Value.Postings.Count;
                termPair.Value.KgramSet = termPair.Key.KGrams(kGramSize);
            }
        }

        public void CalculateScores(DocumentList documentList)
        {
            foreach (var termPair in termDictionary)
            {
                foreach (var posting in termPair.Value.Postings)
                {
                    if (posting.Value.TermFrequency > 0)
                    {
                        var value = (1 + Math.Log10(posting.Value.TermFrequency)) *
                                    Math.Log(documentList.DocumentCount / (double)termPair.Value.DocumentFrequency);
                        posting.Value.Weight = value;
                    }
                    else
                    {
                        posting.Value.Weight = 0;
                    }
                }
            }
        }

        public List<string> GetTermsByQuery(string query, KGramList kGramList, int kGram, int r)
        {
            List<string> words = query.ToLower().Split(' ').ToList();
            List<string> searchTerms = new List<string>();

            foreach (var word in words)
            {
                bool found = this.TryGetTermByName(word, out Term foundTerm);
                if (found == false || foundTerm.Postings.Count < r)
                {
                    Console.WriteLine("Suppose the term {0} is miswritten", word);
                    Console.WriteLine("- Candidates are: ");

                    var candidates = kGramList.GetWords(word, kGram);
                    foreach (var cand in candidates)
                    {
                        Console.WriteLine(cand);
                    }
                    searchTerms.AddRange(candidates);
                }
                else
                {
                    searchTerms.Add(word);
                }
            }

            return searchTerms;
        }
    }
}