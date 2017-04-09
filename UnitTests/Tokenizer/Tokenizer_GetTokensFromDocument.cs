using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Castle.Core.Configuration;
using FluentAssertions;
using InformationRetrieval.Tokenizer;
using NSubstitute;
using Xunit;

namespace UnitTests
{
    public class Tokenizer_GetTokensFromDocument
    {
        [Fact]
        public void Tokenizer_GetTokensFromDocument_Positive()
        {
            var tokenizer = new Tokenizer();

            string documentContent =
                @"Die Frau hatte zwei Töchter.";

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            var referenceTokenList = new List<Token>();
            referenceTokenList.AddRange(new List<Token>
            {
                new Token("die"),
                new Token("frau"),
                new Token("hatte"),
                new Token("zwei"),
                new Token("töchter"),
            });

            tokenList.ShouldAllBeEquivalentTo(referenceTokenList);
        }

        [Fact]
        public void Tokenizer_GetTokensFromDocument_TestDelimiters()
        {
            var tokenizer = new Tokenizer();

            string documentContent =
                "\"Baum\", sprach Jan, rüttel' dich.";

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            var referenceTokenList = new List<Token>();
            referenceTokenList.AddRange(new List<Token>
            {
                new Token("baum"),
                new Token("sprach"),
                new Token("jan"),
                new Token("rüttel'"),
                new Token("dich"),
            });

            tokenList.ShouldAllBeEquivalentTo(referenceTokenList);
        }

        [Fact]
        public void Tokenizer_GetTokensFromDocument_LineSeparation()
        {
            var tokenizer = new Tokenizer();

            string documentContent =
                @"Mordor: 
                    Der Ring gehört uns!";

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            var referenceTokenList = new List<Token>();
            referenceTokenList.AddRange(new List<Token>
            {
                new Token("mordor"),
                new Token("der"),
                new Token("ring"),
                new Token("gehört"),
                new Token("uns"),
            });

            tokenList.ShouldAllBeEquivalentTo(referenceTokenList);
        }

        [Fact]
        public void Tokenizer_GetTokensFromDocument_NegativeTest()
        {
            var tokenizer = new Tokenizer();

            string documentContent = string.Empty;

            var tokenList = tokenizer.GetTokensFromDocument(documentContent);

            tokenList.Should().BeEmpty();
        }
    }
}
