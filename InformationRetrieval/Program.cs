/*
(c) 2019 by Florian Scholz
This file is part of InformationRetrieval.

    InformationRetrieval is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    InformationRetrieval is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with InformationRetrieval.  If not, see <http://www.gnu.org/licenses/>.
*/

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

            bool is_boolean = false;
            while (true)
            {
                SortedSet<Posting> documents = null;
                if (is_boolean == true)
                {
                    Console.Write("Boolean: ");
                    string request = Console.ReadLine().Trim('\r', '\n').Trim();
                    if (request == "switch")
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

                    //var documents = queryProcessor.EvaluateFullPhraseQuery(request, index);
                    documents = queryProcessor.EvaluateFullPhraseQuery(request, index);
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
                        Console.WriteLine(doc.Document);
                    }
                    Console.WriteLine("---");
                }
            }

        }
    }
}