using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InformationRetrieval.Index;
using InformationRetrieval.Tokenizer;
using MathNet.Numerics.Statistics;
using SimpleInjector;

namespace InformationRetrieval
{
    public class VectorSearchEngine
    {
        private IIndex index;
        private ITokenizer tokenizer;

        public int KGramSize { get; set; } = 5;
        public int K { get; set; } = 5;
        public int B1 { get; set; } = 5;
        public int B2 { get; set; } = 5;
        public int r { get; set; } = 1;
        public double JaccardLimit { get; set; } = 0.1;
        public int LevenshteinLimit { get; set; } = 9;

        public VectorSearchEngine(Container diContainer, string path)
        {
            this.index = diContainer.GetInstance<IIndex>();
            this.tokenizer = diContainer.GetInstance<ITokenizer>();

            this.readAllDocuments(path);
            this.offlineProcessing();
        }

        private void offlineProcessing()
        {
            index.Finish();
        }

        private void readAllDocuments(string path)
        {
            var directory = Directory.EnumerateFiles(path);

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

            Console.WriteLine("Duration of the scanning process: {0}", sw.Elapsed.TotalSeconds);
        }

        public List<Hit> SearchDocuments(string query, IQueryFlags queryFlags)
        {
            return index.PerformSearch(query, queryFlags, r);
        }
    }
}