using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using InformationRetrieval.ExpressionParser;
using InformationRetrieval.Index;
using NSubstitute;
using Xunit;

namespace UnitTests.QueryProcessor
{
    public class QueryProcessor_EvaluateExpression
    {
        [Fact]
        public void QueryProcessor_EvaluateExpression_Case01()
        {
            var expressionParser = Substitute.For<IExpressionParser>();
            var indexStorage = Substitute.For<IIndex>();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            queryProcessor.EvaluateExpression("|Hexe,König|", indexStorage);

            expressionParser.Received().ParseExpression("|Hexe,König|");
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_Case02()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            var indexStorage = new InformationRetrieval.Index.Index();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var tokensDocA = tokenizer.GetTokensFromDocument("Es war einmal ein kleiner Troll");
            var tokensDocB = tokenizer.GetTokensFromDocument("Es war einmal ein Kater");

            indexStorage.InsertPostings(tokensDocA, "A");
            indexStorage.InsertPostings(tokensDocB, "B");

            var documents = queryProcessor.EvaluateExpression("|einmal,ein|", indexStorage);

            SortedSet<Posting> referenceDocuments = new SortedSet<Posting>()
            {
                new Posting("A"),
                new Posting("B")
            };

            documents.ShouldBeEquivalentTo(referenceDocuments);
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_Case021()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            var indexStorage = new InformationRetrieval.Index.Index();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var tokensDocA = tokenizer.GetTokensFromDocument("Es war einmal ein kleiner Troll");
            var tokensDocB = tokenizer.GetTokensFromDocument("Es war einmal ein Kater");

            indexStorage.InsertPostings(tokensDocA, "A");
            indexStorage.InsertPostings(tokensDocB, "B");

            var documents = queryProcessor.EvaluateExpression("|Troll|", indexStorage);

            SortedSet<Posting> referenceDocuments = new SortedSet<Posting>()
            {
                new Posting("A"),
            };

            documents.ShouldBeEquivalentTo(referenceDocuments);
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_Case03()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            var indexStorage = new InformationRetrieval.Index.Index();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var tokensDocA = tokenizer.GetTokensFromDocument("Es war einmal ein kleiner Troll");
            var tokensDocB = tokenizer.GetTokensFromDocument("Es war einmal ein Kater");

            indexStorage.InsertPostings(tokensDocA, "A");
            indexStorage.InsertPostings(tokensDocB, "B");

            var documents = queryProcessor.EvaluateExpression("|einmal,troll|", indexStorage);

            SortedSet<Posting> referenceDocuments = new SortedSet<Posting>()
            {
                new Posting("A"),
                new Posting("B")
            };

            documents.ShouldBeEquivalentTo(referenceDocuments);
        }


        [Fact]
        public void QueryProcessor_EvaluateExpression_ExpressionEmpty()
        {
            var expressionParser = Substitute.For<IExpressionParser>();
            var indexStorage = Substitute.For<IIndex>();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var sortingList = queryProcessor.EvaluateExpression(string.Empty, indexStorage);

            sortingList.Should().BeEmpty();
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_ExpressionNull()
        {
            var expressionParser = Substitute.For<IExpressionParser>();
            var indexStorage = Substitute.For<IIndex>();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var sortingList = queryProcessor.EvaluateExpression(null, indexStorage);

            sortingList.Should().BeEmpty();
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_IndexNull()
        {
            var expressionParser = Substitute.For<IExpressionParser>();
            Substitute.For<IIndex>();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var sortingList = queryProcessor.EvaluateExpression("König", null);

            sortingList.Should().BeEmpty();
        }
    }
}
