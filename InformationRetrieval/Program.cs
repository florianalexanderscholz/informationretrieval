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

            foreach (var filepath in directory)
            {
                var fileContent = File.ReadAllText(filepath);
                var tokenizedFileContent = tokenizer.GetTokensFromDocument(fileContent);
                index.InsertPostings(tokenizedFileContent, filepath);
            }

            index.Finish();

            bool is_boolean = false;
            while (true)
            {
                List<Posting> documents = null;
                if (is_boolean == true)
                {
                    Console.Write("Boolean: ");
                    string request = Console.ReadLine().Trim('\r', '\n').Trim();
                    if (request == "makefuzzy")
                    {
                        Console.WriteLine("Switching to fuzzy mode!");
                        queryProcessor = diContainer.GetInstance<FuzzyQueryProcessor>();
                        continue;
                    }
                    else if (request == "switch")
                    {
                        is_boolean = !is_boolean;
                        continue;
                    }

                    //var documents = queryProcessor.EvaluateFullPhraseQuery(request, index);
                    documents = queryProcessor.EvaluateBooleanExpression(request, index);
                }
                else if (is_boolean == false)
                {
                    Console.Write("FullPhrase: ");
                    string request = Console.ReadLine().Trim('\r', '\n').Trim();
                    if (request == "switch")
                    {
                        is_boolean = !is_boolean;
                        continue;
                    }

                    documents = queryProcessor.EvaluateFullPhraseQuery(request, index);
                    //documents = queryProcessor.EvaluateFullPhraseQuery(request, index);
                }

                
                if (documents.Any() == false)
                {
                    Console.WriteLine("Keine Treffer");
                }
                else
                {
                    Console.WriteLine("Treffer: ");
                    foreach (var doc in documents)
                    {
                        Console.WriteLine("Doc: {0}, Score: {1}", doc.Document,doc.Score);
                    }
                    Console.WriteLine("---");
                }
                
            }

        }
    }
}