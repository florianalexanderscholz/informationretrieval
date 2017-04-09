using System;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;
using InformationRetrieval.Tokenizer;
using SimpleInjector;

namespace InformationRetrieval
{
    class Program
    {
        static void Main(string[] args)
        {
            Container diContainer = new Container();
            diContainer.RegisterSingleton<IIndex, Index.Index>();
            diContainer.RegisterSingleton<IExpressionParser,ExpressionParser.ExpressionParser>();
            diContainer.RegisterSingleton<ITokenizer, Tokenizer.Tokenizer>();
            diContainer.Verify();

            IIndex index = diContainer.GetInstance<IIndex>();

        }
    }
}