using System.Collections.Generic;
using System.Collections.Immutable;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    public class Index : IIndex
    {
        public SortedList<string, Term> Terms { get;  } = new SortedList<string, Term>();
        public SortedSet<Posting> AllDocuments { get; set; } = new SortedSet<Posting>();
        public void InsertPostings(List<Token> tokens, string filename)
        {
            if (tokens == null || string.IsNullOrEmpty(filename))
            {
                return;
            }

            AllDocuments.Add(new Posting(filename));

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

                term.Postings.Add(new Posting(filename));

                if (found_term == false)
                {
                    Terms.Add(token.Value, term);
                }
            }
        }

        public SortedSet<Posting> GetAllDocuments()
        {
            return new SortedSet<Posting>(AllDocuments);
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