﻿using System.Collections.Generic;
using System.Linq;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    public class DocumentList
    {
        private Dictionary<string, Document> documentDictionary = new Dictionary<string, Document>();
        public void AddDocument(string filename, int docId, List<Token> tokens)
        {
            documentDictionary.Add(filename, new Document(docId)
            {
                Filename = filename,
                Length = tokens.Count
            });
        }

        public void AddTermToDocument(string document, Term term)
        {
            documentDictionary[document].Terms.Add(term);
        }

        public int DocumentCount
        {
            get => documentDictionary.Count;
        }

        public List<Document> Documents
        {
            get => documentDictionary.Values.ToList(); 
        }

        public double CalculateScore(TermList termList, Document document, List<string> correctedWords, int r)
        {
            double score = 0.0;
            foreach (var term in correctedWords)
            {
                bool found = termList.TryGetTermByName(term, out Term foundTerm);
                if (found == false || foundTerm.Postings.Count < r)
                {
                    continue;
                }
                if (foundTerm.Postings.TryGetValue(document.DocId, out Posting posting))
                {
                    score += posting.Weight;
                }
                else
                {
                    score += 0;
                }
            }

            return score / document.Length;
        }
    }
}