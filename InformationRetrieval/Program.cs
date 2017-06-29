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

            IIndex index = diContainer.GetInstance<IIndex>();
            ITokenizer tokenizer = diContainer.GetInstance<ITokenizer>();
            IQueryProcessor queryProcessor = diContainer.GetInstance<IQueryProcessor>();
            readDocumentsToIndex(tokenizer, index);

            while (true)
            {
                Console.Write("Querya: ");
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
                        B1 = 10,
                        B2 = 10
                    };

                    query = readline.Substring(0);
                }

                List<Hit> hitList = index.PerformSearch(query, queryFlags, 1);

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

        private static void readDocumentsToIndex(ITokenizer tokenizer, IIndex index)
        {
            var directory = Directory.EnumerateFiles(@"C:\mantxt");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int docId = 0;
            foreach (var filepath in directory)
            {
                var fileContent = File.ReadAllText(filepath);
                var tokenizedFileContent = tokenizer.GetTokensFromDocument(fileContent);
                index.InsertPostings(tokenizedFileContent, filepath, docId++);
            }
            sw.Stop();
            Console.WriteLine("Tokenization: {0}", sw.Elapsed.TotalSeconds);
            index.Finish();
        }
    }
}