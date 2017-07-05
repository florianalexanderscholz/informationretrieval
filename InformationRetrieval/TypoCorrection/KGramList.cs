using System.Collections.Generic;
using System.Linq;
using InformationRetrieval.Index;
using InformationRetrieval.Utils;

namespace InformationRetrieval.TypoCorrection
{
    public class KGramList
    {
        public Dictionary<string, KGramTerm> kgramDictionary = new Dictionary<string, KGramTerm>();

        public void AddTermToTypoCorrection(Term iTerm, int k)
        {
            var kgrams = iTerm.Name.KGrams(k);

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

                if (term.Words.Keys.Contains(iTerm.Name) == false)
                {
                    term.Words.Add(iTerm.Name, iTerm);
                }

                if (found_term == false)
                {
                    kgramDictionary.Add(kgram, term);
                }
            }
        }

        public List<string> GetWords(string token, int k, double j, int l)
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
                    if (Jaccard(kgrams, term.Value.KgramSet) < j)
                    {
                        continue;
                    }

                    var levensthein = LevenshteinDistance.Compute(term.Value.Name, token);
                    if (levensthein < l)
                    {
                        termList.Add(term.Value.Name);
                    }
                }
            }

            return termList.ToList();
        }
 
        public void Prepare()
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