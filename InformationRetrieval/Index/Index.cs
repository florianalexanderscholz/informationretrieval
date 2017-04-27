using System.Collections.Generic;
using System.Collections.Immutable;
using InformationRetrieval.Tokenizer;
using MathNet.Numerics.LinearAlgebra.Double;

namespace InformationRetrieval.Index
{
    public class Index : IIndex
    {
        private SparseMatrix sparseMatrix = new SparseMatrix(0, 0);
        public SortedList<string, Term> Terms { get;  } = new SortedList<string, Term>();
        public SortedSet<Posting> AllDocuments { get; set; } = new SortedSet<Posting>();
        public void InsertPostings(List<Token> tokens, string filename)
        {
            if (tokens == null || string.IsNullOrEmpty(filename))
            {
                return;
            }

            AllDocuments.Add(new Posting(filename));

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
            return new SortedSet<Posting>(AllDocuments);
        }

        /// <summary>
        /// Calculates Jaccard
        /// </summary>
        public void CalculateJaccard()
        {
            this.sparseMatrix = new SparseMatrix(this.AllDocuments.Count, Terms.Count);
            
            /* Documents */
            for (int i = 0; i < sparseMatrix.RowCount; i++)
            {
                /* Terms */
                for (int j = 0; j < sparseMatrix.ColumnCount; j++)
                {
                    sparseMatrix[i, j] = 1.0;
                }
            }
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
    }
}