using System;
using System.IO;
using System.Linq;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;
using InformationRetrieval.QueryProcessor;
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
            diContainer.RegisterSingleton<IQueryProcessor, QueryProcessor.QueryProcessor>();
            diContainer.Verify();

            IIndex index = diContainer.GetInstance<IIndex>();
            ITokenizer tokenizer = diContainer.GetInstance<ITokenizer>();
            IQueryProcessor queryProcessor = diContainer.GetInstance<IQueryProcessor>();
            var directory = Directory.EnumerateFiles("../Documents/Corpus");

            foreach (var filepath in directory)
            {
                var fileContent = File.ReadAllText(filepath);
                var tokenizedFileContent = tokenizer.GetTokensFromDocument(fileContent);
                index.InsertPostings(tokenizedFileContent, filepath);
            }

            while (true)
            {
                Console.Write("Abfrage: ");
                string request = Console.ReadLine().Trim('\r', '\n').Trim();

                var documents = queryProcessor.EvaluateExpression(request, index);

                if (documents.Any() == false)
                {
                    Console.WriteLine("Keine Treffer");
                }
                else
                {
                    Console.WriteLine("Treffer: ");
                    foreach (var doc in documents)
                    {
                        Console.WriteLine(doc.Document);
                    }
                    Console.WriteLine("---");
                }
            }

        }
    }
}