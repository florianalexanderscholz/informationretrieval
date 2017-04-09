using System;
using System.Collections.Generic;
using System.Text;

namespace InformationRetrieval.Index
{
    public class Posting
    {
        public Posting(string document)
        {
            this.Document = document;
        }

        public string Document { get; set; }
    }
}
