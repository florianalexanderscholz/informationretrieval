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

            List<Token> tokenList = new List<Token>();
            foreach (var token in tokenSet)
            {
                if (token.Length > 0)
                {
                    var processedToken = token.ToLower();
                    tokenList.Add(new Token(processedToken));
                }
            }

            return tokenList;
        }
    }
}