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

            int b1 = 2;
            int b2 = 2;
            int k = 10;;
            int r = 1;
            int l = 5;
            double j = 0.1;

            string corpusPath = @"C:\Users\Florian\Downloads\mantxt.tar\mantxt";
            VectorSearchEngine vectorSearchEngine = new VectorSearchEngine(diContainer, corpusPath, b1, b2);

            while (true)
            {
                Console.Write("Query: ");
                string readline = Console.ReadLine();


                if (readline.StartsWith("flags"))
                {
                    var tokens = readline.Split();
                    if (tokens.Length < 7)
                    {
                        Console.WriteLine("flags: not enough parameters");
                        Console.WriteLine("flags (current) B1 <- {0}, B2 <- {1}, K <- {2}, R <- {3}, L <- {4}, J <- {5}.", b1, b2, k, r, l, j);
                        Console.WriteLine("flags (default): 2 2 5 3 3 0,1");
                        continue;
                    }
                    else
                    {
                        var _b1 = Convert.ToInt32(tokens[1]);
                        var _b2 = Convert.ToInt32(tokens[2]);
                        var _k = Convert.ToInt32(tokens[3]);
                        var _r = Convert.ToInt32(tokens[4]);
                        var _l = Convert.ToInt32(tokens[5]);
                        var _j = Convert.ToDouble(tokens[6]);

                        bool need_refresh = false;
                        if (b1 != _b1)
                        {
                            need_refresh = true;
                        }
                        b1 = _b1;
                        b2 = _b2;
                        k = _k;
                        r = _r;
                        j = _j;
                        l = _l;
                        Console.WriteLine("Changed B1 <- {0}, B2 <- {1}, K <- {2}, R <- {3}, L <- {4}, J <- {5}.",b1, b2, k, r,l, j);
                        if (need_refresh)
                        {
                            vectorSearchEngine.RefreshClustering(b1);
                            Console.WriteLine("Recreated the Clusters.");
                        }
                        else
                        {
                            Console.WriteLine("The clusters are not rebuilt.");
                        }
                        continue;
                    }
                }

                string query;

                IQueryFlags queryFlags = null;
                if (readline.StartsWith("!"))
                {
                    queryFlags = new VectorSearchFlags()
                    {
                        K = k,
                        R = r,
                        L = l,
                        J = j
                    };
                    query = readline.Substring(1);
                }
                else
                {
                    queryFlags = new ClusterSearchFlags()
                    {
                        B1 = b1,
                        B2 = b2,
                        K = k,
                        R = r,
                        L = l,
                        J = j
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