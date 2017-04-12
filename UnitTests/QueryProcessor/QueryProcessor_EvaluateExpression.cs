using System.Collections.Generic;
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
        public void QueryProcessor_EvaluateExpression_AbstractTest()
        {
            var expressionParser = Substitute.For<IExpressionParser>();
            var indexStorage = Substitute.For<IIndex>();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            expressionParser.ParseExpression("|Hexe,König|").Returns(new DNFExpression());

            queryProcessor.EvaluateExpression("|Hexe,König|", indexStorage);

            expressionParser.Received().ParseExpression("|Hexe,König|");
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_AndConjunction()
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
        public void QueryProcessor_EvaluateExpression_SingleSearch()
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
        public void QueryProcessor_EvaluateExpression_AndNotWithoutOr()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            var indexStorage = new InformationRetrieval.Index.Index();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var tokensDocA = tokenizer.GetTokensFromDocument("Es war einmal ein kleiner Troll");
            var tokensDocB = tokenizer.GetTokensFromDocument("Es war einmal ein Kater");

            indexStorage.InsertPostings(tokensDocA, "A");
            indexStorage.InsertPostings(tokensDocB, "B");

            var documents = queryProcessor.EvaluateExpression("|ein,!Troll|", indexStorage);

            SortedSet<Posting> referenceDocuments = new SortedSet<Posting>()
            {
                new Posting("B"),
            };

            documents.ShouldBeEquivalentTo(referenceDocuments);
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_OrOr()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            var indexStorage = new InformationRetrieval.Index.Index();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var tokensDocA = tokenizer.GetTokensFromDocument("Es war einmal ein kleiner Troll");
            var tokensDocB = tokenizer.GetTokensFromDocument("Es war einmal ein Kater");

            indexStorage.InsertPostings(tokensDocA, "A");
            indexStorage.InsertPostings(tokensDocB, "B");

            var documents = queryProcessor.EvaluateExpression("|Troll|Kater|", indexStorage);

            SortedSet<Posting> referenceDocuments = new SortedSet<Posting>()
            {
                new Posting("A"),
                new Posting("B")
            };

            documents.ShouldBeEquivalentTo(referenceDocuments);
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_SingleAndInOr()
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
            };

            documents.ShouldBeEquivalentTo(referenceDocuments);
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_AndOrAndNot()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            var indexStorage = new InformationRetrieval.Index.Index();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var tokensDocA = tokenizer.GetTokensFromDocument("Es war einmal ein kleiner Troll");
            var tokensDocB = tokenizer.GetTokensFromDocument("Es war einmal ein Kater");
            var tokensDocC = tokenizer.GetTokensFromDocument("Es war einmal ein Fuchs");
            indexStorage.InsertPostings(tokensDocA, "A");
            indexStorage.InsertPostings(tokensDocB, "B");
            indexStorage.InsertPostings(tokensDocC, "C");
            var documents = queryProcessor.EvaluateExpression("|Kater|es,!fuchs|", indexStorage);

            SortedSet<Posting> referenceDocuments = new SortedSet<Posting>()
            {
                new Posting("A"),
                new Posting("B")
            };

            documents.ShouldBeEquivalentTo(referenceDocuments);
        }

        [Fact]
        public void QueryProcessor_EvaluateExpression_Not()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();
            var tokenizer = new InformationRetrieval.Tokenizer.Tokenizer();
            var indexStorage = new InformationRetrieval.Index.Index();
            var queryProcessor = new InformationRetrieval.QueryProcessor.QueryProcessor(expressionParser);

            var tokensDocA = tokenizer.GetTokensFromDocument("Es war einmal ein kleiner Troll");
            var tokensDocB = tokenizer.GetTokensFromDocument("Es war einmal ein Kater");
            var tokensDocC = tokenizer.GetTokensFromDocument("Es war einmal ein Fuchs");
            indexStorage.InsertPostings(tokensDocA, "A");
            indexStorage.InsertPostings(tokensDocB, "B");
            indexStorage.InsertPostings(tokensDocC, "C");
            var documents = queryProcessor.EvaluateExpression("|!Fuchs|", indexStorage);

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
