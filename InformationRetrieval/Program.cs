using System;
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
            diContainer.RegisterSingleton<IIndex, Index.Index>();
            diContainer.RegisterSingleton<ITokenizer, Tokenizer.Tokenizer>();
            diContainer.RegisterSingleton<IQueryProcessor, QueryProcessor.QueryProcessor>();
            diContainer.Verify();

            IIndex index = diContainer.GetInstance<IIndex>();
            ITokenizer tokenizer = diContainer.GetInstance<ITokenizer>();
            IQueryProcessor queryProcessor = diContainer.GetInstance<IQueryProcessor>();
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

            while (true)
            {
                Console.Write("Querya: ");
                string readline = Console.ReadLine();
                index.PerformSearch(readline);
            }
        }
    }
}