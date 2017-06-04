using System.Collections.Generic;
namespace InformationRetrieval.Tokenizer
{
    public interface ITokenizer
    {
        List<Token> GetTokensFromDocument(string documentContent);
    }
}