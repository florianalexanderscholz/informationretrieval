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

namespace InformationRetrieval.Index
{
    /// <summary>
    /// Implements a Posting
    /// </summary>
    public class Posting : IComparable
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="document">Document that corresponds to the Posting</param>
        public Posting(string document)
        {
            this.Document = document;
        }

        /// <summary>
        /// A SortedSet of the Positions (Ascending)
        /// </summary>
        public SortedSet<int> Positions { get; set; } = new SortedSet<int>();

        /// <summary>
        /// The corresponding Document
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// Compareto Implementation
        /// </summary>
        /// <param name="obj">Type of Posting (String.Compare for Document property)</param>
        /// <returns>String.Compare...</returns>
        public int CompareTo(object obj)
        {
            Posting b = (Posting) obj;
            return String.Compare(Document, b.Document, StringComparison.OrdinalIgnoreCase);
        }
    }
}
