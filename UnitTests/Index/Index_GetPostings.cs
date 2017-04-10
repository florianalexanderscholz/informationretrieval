using System.Collections.Generic;
using FluentAssertions;
using InformationRetrieval.Index;
using InformationRetrieval.Tokenizer;
using Xunit;

namespace UnitTests.Index
{
    public class Index_GetPostings
    {
        [Fact]
        public void Index_GetPostings_NotFound()
        {
            InformationRetrieval.Index.Index index = new InformationRetrieval.Index.Index();

            List<Token> tokens = new List<Token>()
            {
                new Token("ali"),
                new Token("baba"),
                new Token("die"),
                new Token("und")
            };

            index.InsertPostings(tokens, "Ali Baba und die 40 Räuber.txt");

            bool found = index.GetPosting("sesam", out Term term);
            found.Should().BeFalse();
        }

        [Fact]
        public void Index_GetPostings_TokenIsEmpty()
        {
            InformationRetrieval.Index.Index index = new InformationRetrieval.Index.Index();

            List<Token> tokens = new List<Token>()
            {
                new Token("ali"),
                new Token("baba"),
                new Token("die"),
                new Token("und")
            };

            index.InsertPostings(tokens, "Ali Baba und die 40 Räuber.txt");

            bool found = index.GetPosting(string.Empty, out Term term);
            found.Should().BeFalse();
        }

        [Fact]
        public void Index_GetPostings_TokenFound()
        {
            InformationRetrieval.Index.Index index = new InformationRetrieval.Index.Index();

            List<Token> tokens = new List<Token>()
            {
                new Token("ali"),
                new Token("baba"),
                new Token("die"),
                new Token("und")
            };

            index.InsertPostings(tokens, "Ali Baba und die 40 Räuber.txt");

            bool found = index.GetPosting("ali", out Term term);
            found.Should().BeTrue();

            term.Postings.ShouldBeEquivalentTo(new SortedSet<Posting>()
            {
                new Posting("Ali Baba und die 40 Räuber.txt")
            });
        }
    }
}