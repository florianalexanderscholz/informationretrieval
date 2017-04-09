using System.Collections.Generic;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    public class Index : IIndex
    {
        public SortedList<string, Term> Terms { get;  } = new SortedList<string, Term>();

        public void InsertPostings(List<Token> tokens, string filename)
        {
            if (tokens == null || string.IsNullOrEmpty(filename))
            {
                return;
            }

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
    }
}