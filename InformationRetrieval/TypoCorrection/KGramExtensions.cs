using System;
using System.Collections.Generic;
using InformationRetrieval.QueryProcessor;

namespace InformationRetrieval.TypoCorrection
{
    public static class KGramExtensions
    {
        public static SortedSet<string> KGrams(this string word, int k)
        {
            string input = "$" + word + "$";
            int i = 0;
            int j;

            SortedSet<string> kgrams = new SortedSet<string>(new DuplicateKeyComparer<string>());

            do
            {
                string kgram = input.Substring(i, Math.Min(input.Length - i, k));
                i++;
                kgrams.Add(kgram);
            } while (i < word.Length);

            return kgrams;
        }
    }
}