﻿using System.Collections.Generic;
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
                new Token("ali", 0),
                new Token("baba", 1),
                new Token("die", 2),
                new Token("und", 3)
            };

            index.InsertPostings(tokens, "Ali Baba und die 40 Räuber.txt");

            var dictionary = index.Terms;

            SortedList<string, Term> referenceDictionary = new SortedList<string, Term>();
            referenceDictionary.Add("ali", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt"){ Positions = {0}}
                }
            });
            referenceDictionary.Add("baba", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt") { Positions = {1}}
                }
            });
            referenceDictionary.Add("die", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt") {Positions = {2}}
                }
            });
            referenceDictionary.Add("und", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt") {Positions = {3}}
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
                new Token("ali", 0),
                new Token("baba", 1),
                new Token("die", 2),
                new Token("und", 3)
            };

            index.InsertPostings(tokens, "Ali Baba und die 40 Räuber.txt");

            List<Token> otherTokens = new List<Token>()
            {
                new Token("ali", 0),
                new Token("baba", 1)
            };

            index.InsertPostings(otherTokens, "Romeo und Julia.txt");

            var dictionary = index.Terms;

            SortedList<string, Term> referenceDictionary = new SortedList<string, Term>();
            referenceDictionary.Add("ali", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                    {
                        Positions = {0}
                    },
                    new Posting("Romeo und Julia.txt")
                    {
                        Positions = {0}
                    }
                }
            });
            referenceDictionary.Add("baba", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                    {
                        Positions = {1}
                    },
                    new Posting("Romeo und Julia.txt")
                    {
                        Positions = {1}
                    }
                }
            });
            referenceDictionary.Add("die", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                    {
                        Positions = {2}
                    }
                }
            });
            referenceDictionary.Add("und", new Term()
            {
                Postings = new SortedSet<Posting>()
                {
                    new Posting("Ali Baba und die 40 Räuber.txt")
                    {
                        Positions = {3}
                    }
                }
            });

            dictionary.ShouldBeEquivalentTo(referenceDictionary);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Index_InsertPostings_FilenameNull(string filename)
        {
            InformationRetrieval.Index.Index index = new InformationRetrieval.Index.Index();

            index.InsertPostings(new List<Token>(), filename);

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