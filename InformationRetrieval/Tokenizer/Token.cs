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

namespace InformationRetrieval.Tokenizer
{
    /// <summary>
    /// Token DTO
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">A value string</param>
        public Token(string value, int documentPosition)
        {
            this.Value = value;
            this.Position = documentPosition;
        }

        /// <summary>
        /// Retrieves the string representation.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The position of the term in a document
        /// </summary>
        public int Position { get; set; }
    }
}