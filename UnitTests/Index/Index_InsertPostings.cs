using System.Collections.Generic;
using FluentAssertions;
using InformationRetrieval.Index;
using InformationRetrieval.Tokenizer;
using Xunit;

namespace UnitTests.Index
{
    public class Index_InsertPostings
    {
        [Fact]
        public void Index_InsertPostings_InsertUniqueValues()
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

            var dictionary = index.Terms;

            SortedList<string, Term> referenceDictionary = new SortedList<string, Term>();
            referenceDictionary.Add("ali", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                }
            });
            referenceDictionary.Add("baba", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                }
            });
            referenceDictionary.Add("die", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                }
            });
            referenceDictionary.Add("und", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                }
            });

            dictionary.ShouldBeEquivalentTo(referenceDictionary);
        }

        [Fact]
        public void Index_InsertPostings_InsertMultipleValues()
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

            List<Token> otherTokens = new List<Token>()
            {
                new Token("ali"),
                new Token("baba")
            };

            index.InsertPostings(otherTokens, "Romeo und Julia.txt");

            var dictionary = index.Terms;

            SortedList<string, Term> referenceDictionary = new SortedList<string, Term>();
            referenceDictionary.Add("ali", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt"),
                    new Posting("Romeo und Julia.txt")
                }
            });
            referenceDictionary.Add("baba", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt"),
                    new Posting("Romeo und Julia.txt")
                }
            });
            referenceDictionary.Add("die", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                }
            });
            referenceDictionary.Add("und", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                }
            });

            dictionary.ShouldBeEquivalentTo(referenceDictionary);
        }

        [Fact]
        public void Index_InsertPostings_FilenameNull()
        {
            InformationRetrieval.Index.Index index = new InformationRetrieval.Index.Index();

            index.InsertPostings(new List<Token>(), null);

            index.Terms.Should().BeEmpty();
        }

        [Fact]
        public void Index_InsertPostings_FilenameEmpty()
        {
            InformationRetrieval.Index.Index index = new InformationRetrieval.Index.Index();

            index.InsertPostings(new List<Token>(), string.Empty);

            index.Terms.Should().BeEmpty();
        }

        [Fact]
        public void Index_InsertPostings_TokensNull()
        {
            InformationRetrieval.Index.Index index = new InformationRetrieval.Index.Index();

            index.InsertPostings(null, "filename");

            index.Terms.Should().BeEmpty();
        }
    }
}