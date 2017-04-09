using System;
using System.Collections.Generic;
using System.Text;
using InformationRetrieval.Tokenizer;

namespace InformationRetrieval.Index
{
    interface IIndex
    {
        void InsertPostings(List<Token> tokens, string filename);
    }
}
