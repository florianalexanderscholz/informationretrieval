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

namespace InformationRetrieval.Tokenizer
{
    /// <summary>
    /// Tokenizer Implementation for P1
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        /// <summary>
        /// Tokenizes a document as described by Klaus Weidenhaupt. 
        /// </summary>
        /// <param name="documentContent">The string representation of a document.</param>
        /// <returns>A list of tokens. If documentContent is null or empty, an empty list is returned.</returns>
        public List<Token> GetTokensFromDocument(string documentContent)
        {
            if (string.IsNullOrEmpty(documentContent))
            {
                return new List<Token>();
            }

            var tokenSet = documentContent.Split(new char[] {'.', ',', ';', ':', '!', '?', '"', '-', ' ', '\n', '\r', '\t'});

            int wordCounter = 0;

            List<Token> tokenList = new List<Token>();
            foreach (var token in tokenSet)
            {
                if (token.Length > 0)
                {
                    var processedToken = token.ToLower();
                    processedToken = processedToken.Replace("ö","oe");
                    processedToken = processedToken.Replace("ä", "ae");
                    processedToken = processedToken.Replace("ü", "ue");
                    tokenList.Add(new Token(processedToken, wordCounter));
                    wordCounter++;
                }
            }

            return tokenList;
        }
    }
}