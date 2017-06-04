using System;
using System.Collections.Generic;
using System.Text;
using InformationRetrieval.Tokenizer;
using MathNet.Numerics.LinearAlgebra.Complex;
using SparseMatrix = MathNet.Numerics.LinearAlgebra.Double.SparseMatrix;

namespace InformationRetrieval.Index
{
    public interface IIndex
    {
        void InsertPostings(List<Token> tokens, string filename, int docId);
        SortedSet<Posting> GetAllDocuments();
        bool GetPosting(string token, out Term term);
        void PerformSearch(string query);
        void Finish();
    }
}
