using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using InformationRetrieval.PostingListUtils;

namespace InformationRetrieval.Index
{
    public class KGramList
    {
        public Dictionary<string, KGramTerm> kgramDictionary = new Dictionary<string, KGramTerm>();

        public void Add(Term iTerm,string token, int k=3)
        {
            var kgrams = token.KGrams(k);

            iTerm.KgramSet = kgrams;
            foreach (var kgram in kgrams)
            {
                bool found_term = false;
                

                if (kgramDictionary.TryGetValue(kgram, out KGramTerm term) == true)
                {
                    found_term = true;
                }
                else
                {
                    term = new KGramTerm()
                    {
                        KGram = kgram
                    };
                }

                if (term.Words.Keys.Contains(token) == false)
                {
                    term.Words.Add(token, iTerm);
                }

                if (found_term == false)
                {
                    kgramDictionary.Add(kgram, term);
                }
            }
        }

        public List<string> GetWords(string token, int k)
        {
            var kgrams = token.KGrams(k);
            
            SortedSet<string> termList = new SortedSet<string>();

            foreach (var kgram in kgrams)
            {
                if (kgramDictionary.TryGetValue(kgram, out KGramTerm kgramTerm) == false)
                {
                    continue;
                }

                foreach (var term in kgramTerm.Words)
                {
                    if (Jaccard(kgrams, term.Value.KgramSet) < 0.1)
                    {
                        continue;
                    }

                    var levensthein = LevenshteinDistance.Compute(term.Value.Name, token);
                    if (levensthein < 6)
                    {
                        termList.Add(term.Value.Name);
                    }
                }
            }

            return termList.ToList();
        }
 
        public void Finish()
        {
            int i = 0;
            foreach (var kgram in kgramDictionary)
            {
                kgram.Value.Index = i++;
            }
        }

        private double Jaccard(SortedSet<string> a, SortedSet<string> b)
        {
            double similar = a.AndCount(b);

            return similar / ((a.Count + b.Count) - similar);
        }
    }
}