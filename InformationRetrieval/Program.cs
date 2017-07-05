using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            initializeDi(diContainer);

            int b1 = 3;
            int b2 = 3;

            string corpusPath = @"C:\Users\Florian\Downloads\inrpraktikum\Documents\Corpus";
            VectorSearchEngine vectorSearchEngine = new VectorSearchEngine(diContainer, corpusPath, b1, b2);

            while (true)
            {
                Console.Write("Query: ");
                string readline = Console.ReadLine();
 
                string query;

                IQueryFlags queryFlags = null;
                if (readline.StartsWith("!"))
                {
                    queryFlags = new VectorSearchFlags()
                    {
                        K = 10
                    };
                    query = readline.Substring(1);
                }
                else
                {
                    queryFlags = new ClusterSearchFlags()
                    {
                        B1 = b1,
                        B2 = b2,
                        K = 10
                    };

                    query = readline.Substring(0);
                }

                List<Hit> hitList = vectorSearchEngine.SearchDocuments(query, queryFlags);

                foreach (var hit in hitList)
                {
                    Console.WriteLine("{0} (score: {1})", hit.Document, hit.Score);
                }
            }
        }

        private static void initializeDi(Container diContainer)
        {
            diContainer.RegisterSingleton<IIndex, Index.Index>();
            diContainer.RegisterSingleton<ITokenizer, Tokenizer.Tokenizer>();
            diContainer.RegisterSingleton<IQueryProcessor, QueryProcessor.QueryProcessor>();
            diContainer.Verify();
        }
    }
}