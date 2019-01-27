/*
(c) 2019 by Florian Scholz
This file is part of InformationRetrieval.

    InformationRetrieval is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    InformationRetrieval is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with InformationRetrieval.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Collections.Immutable;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    /// <summary>
    /// Implements the Index
    /// </summary>
    public class Index : IIndex
    {
        /// <summary>
        /// A Sorted List of Terms (Alphabetically)
        /// </summary>
        public SortedList<string, Term> Terms { get;  } = new SortedList<string, Term>();
        
        /// <summary>
        /// A Sorted Set of Postings (Alphabetically)
        /// </summary>
        public SortedSet<Posting> AllDocuments { get; set; } = new SortedSet<Posting>();
        
        /// <summary>
        /// Inserts tokens from a file source into the Index
        /// </summary>
        /// <param name="tokens">List of Tokens</param>
        /// <param name="filename">Filename</param>
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

        /// <summary>
        /// Returns all Documents (Postings)
        /// </summary>
        /// <returns>SortedSet of Postings (Documents)</returns>
        public SortedSet<Posting> GetAllDocuments()
        {
            return new SortedSet<Posting>(AllDocuments);
        }

        /// <summary>
        /// Retrieves a Posting, found by a token
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="term">The term</param>
        /// <returns>True if the Posting existed.</returns>
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