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

using System;
using System.Collections.Generic;
using System.Text;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    /// <summary>
    /// Abstract Interface for the Index
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// Inserts a list of tokens into the Index
        /// </summary>
        /// <param name="tokens">List of tokens</param>
        /// <param name="filename">The file source of the tokens</param>
        void InsertPostings(List<Token> tokens, string filename);
        
        /// <summary>
        /// Returns all documents postings, sorted alphabetically
        /// </summary>
        /// <returns>SortedSet of postings</returns>
        SortedSet<Posting> GetAllDocuments();
 
        /// <summary>
        /// Returns a term, searched by a token
        /// </summary>
        /// <param name="token">(In) Token</param>
        /// <param name="term">(Out) Term</param>
        /// <returns>True if the token exists</returns>
        bool GetPosting(string token, out Term term);
    }
}
