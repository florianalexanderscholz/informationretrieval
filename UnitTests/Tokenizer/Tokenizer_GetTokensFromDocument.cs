using System.Collections.Generic;
using FluentAssertions;
using InformationRetrieval.Tokenizer;
using Xunit;

namespace UnitTests.Tokenizer
{
    public class Tokenizer_GetTokensFromDocument
    {
        [Fact]
        public void Tokenizer_GetTokensFromDocument_Positive()
        {
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();

            const string documentContent = @"Die Frau hatte zwei Töchter.";

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            var referenceTokenList = new List<Token>();
            referenceTokenList.AddRange(new List<Token>
            {
                new Token("die"),
                new Token("frau"),
                new Token("hatte"),
                new Token("zwei"),
                new Token("toechter"),
            });

            tokenList.ShouldAllBeEquivalentTo(referenceTokenList);
        }

        [Fact]
        public void Tokenizer_GetTokensFromDocument_TestDelimiters()
        {
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();

            const string documentContent = "\"Baum\", sprach Jan, ruettel' dich.";

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            var referenceTokenList = new List<Token>();
            referenceTokenList.AddRange(new List<Token>
            {
                new Token("baum"),
                new Token("sprach"),
                new Token("jan"),
                new Token("ruettel'"),
                new Token("dich"),
            });

            tokenList.ShouldAllBeEquivalentTo(referenceTokenList);
        }

        [Fact]
        public void Tokenizer_GetTokensFromDocument_LineSeparation()
        {
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();

            const string documentContent = @"Mordor: 
                    Der Ring gehört uns!";

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            var referenceTokenList = new List<Token>();
            referenceTokenList.AddRange(new List<Token>
            {
                new Token("mordor"),
                new Token("der"),
                new Token("ring"),
                new Token("gehoert"),
                new Token("uns"),
            });

            tokenList.ShouldAllBeEquivalentTo(referenceTokenList);
        }

        [Fact]
        public void Tokenizer_GetTokensFromDocument_IsEmpty()
        {
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();

            string documentContent = string.Empty;

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            tokenList.Should().BeEmpty();
        }

        [Fact]
        public void Tokenizer_GetTokensFromDocument_IsNull()
        {
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            
            var tokenList = tokenizer.GetTokensFromDocument(null);

            tokenList.Should().BeEmpty();
        }
    }
}
