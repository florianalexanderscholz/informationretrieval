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
                new Token("die", 0),
                new Token("frau", 1),
                new Token("hatte", 2),
                new Token("zwei", 3),
                new Token("toechter", 4),
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
                new Token("baum", 0),
                new Token("sprach", 1),
                new Token("jan", 2),
                new Token("ruettel'", 3),
                new Token("dich", 4),
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
                new Token("mordor", 0),
                new Token("der", 1),
                new Token("ring", 2),
                new Token("gehoert", 3),
                new Token("uns", 4),
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
