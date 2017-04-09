using System.Collections.Generic;
using System.Dynamic;

namespace InformationRetrieval.Tokenizer
{
    public interface ITokenizer
    {
        List<Token> GetTokensFromDocument(string documentContent);
    }
}