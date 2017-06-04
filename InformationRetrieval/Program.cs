using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;
using InformationRetrieval.QueryProcessor;
using InformationRetrieval.Tokenizer;
using SimpleInjector;

namespace InformationRetrieval
{
    public class Program
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

            int docId = 0;
            foreach (var filepath in directory)
            {
                var fileContent = File.ReadAllText(filepath);
                var tokenizedFileContent = tokenizer.GetTokensFromDocument(fileContent);
                index.InsertPostings(tokenizedFileContent, filepath, docId++);
            }

            index.Finish();

            while (true)
            {
                Console.Write("Query: ");
                string readline = Console.ReadLine();
                index.PerformSearch(readline);
            }
        }
    }
}