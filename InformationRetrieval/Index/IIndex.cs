using System;
using System.Collections.Generic;
using System.Text;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    public interface IIndex
    {
        void InsertPostings(List<Token> tokens, string filename);
        SortedSet<Posting> GetAllDocuments();
        bool GetPosting(string token, out Term term);
        void Finish();
        double GetFuzzyScore(string document, string term);
    }
}
