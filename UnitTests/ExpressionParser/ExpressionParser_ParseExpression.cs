using System.Collections.Generic;
using FluentAssertions;
using InformationRetrieval.ExpressionParser;
using Xunit;

namespace UnitTests.ExpressionParser
{
    public class ExpressionParser_ParseExpression
    {
        [Fact]
        public void ExpressionParser_ParseExpression_ParseOneAndExpression()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser()
            {
                DelimiterAnd = '&',
                DelimiterOr = '|',
                PrefixNegative = '!'
            };

            var dnfExpression = expressionParser.ParseExpression("|Hexe&Prinzessin&||");

            var referenceBooleanExpression = new DNFExpression()
            {
                Conjunctions = new List<Conjunction>()
                {
                    new Conjunction()
                    {
                        Variables = new List<Variable>()
                        {
                            new Variable("Hexe")
                            {
                                Negative = false
                            },
                            new Variable("Prinzessin")
                            {
                                Negative = false
                            }
                        }
                    }
                }
            };

            dnfExpression.ShouldBeEquivalentTo(referenceBooleanExpression);
        }

        [Fact]
        public void ExpressionParser_ParseExpression_ParseMultipleAndExpressions()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();

            var dnfExpression = expressionParser.ParseExpression("|Hexe,Prinzessin|Frosch,König,Tellerlein|");

            var referenceBooleanExpression = new DNFExpression()
            {
                Conjunctions = new List<Conjunction>()
                {
                    new Conjunction()
                    {
                        Variables = new List<Variable>()
                        {
                            new Variable("Hexe"),
                            new Variable("Prinzessin")
                        }
                    },
                    new Conjunction()
                    {
                        Variables = new List<Variable>()
                        {
                            new Variable("Frosch"),
                            new Variable("König"),
                            new Variable("Tellerlein"),
                        }
                    }
                }
            };

            dnfExpression.ShouldBeEquivalentTo(referenceBooleanExpression);
        }

        [Fact]
        public void ExpressionParser_ParseExpression_ParseMultipleAndExpressionsWithNegativeVariable()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();

            var dnfExpression = expressionParser.ParseExpression("|Hexe,Prinzessin|König,!Hexe||");

            var referenceBooleanExpression = new DNFExpression()
            {
                Conjunctions = new List<Conjunction>()
                {
                    new Conjunction()
                    {
                        Variables = new List<Variable>()
                        {
                            new Variable("Hexe"),
                            new Variable("Prinzessin")
                        }
                    },
                    new Conjunction()
                    {
                        Variables = new List<Variable>()
                        {
                            new Variable("König"),
                            new Variable("Hexe")
                            {
                                Negative = true
                            },
                        }
                    }
                }
            };

            dnfExpression.ShouldBeEquivalentTo(referenceBooleanExpression);
        }
        [Fact]
        public void ExpressionParser_ParseExpression_ParseOneVariable()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();

            var dnfExpression = expressionParser.ParseExpression("Hexe");

            var referenceBooleanExpression = new DNFExpression()
            {
                Conjunctions = new List<Conjunction>()
                {
                    new Conjunction()
                    {
                        Variables = new List<Variable>()
                        {
                            new Variable("Hexe"),
                        }
                    }
                }
            };

            dnfExpression.ShouldBeEquivalentTo(referenceBooleanExpression);
        }

        [Fact]
        public void ExpressionParser_ParseExpression_IsNegative()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();

            var dnfExpression = expressionParser.ParseExpression(string.Empty);

            var referenceBooleanExpression = new DNFExpression()
            {
            };

            dnfExpression.ShouldBeEquivalentTo(referenceBooleanExpression);
        }

        [Fact]
        public void ExpressionParser_ParseExpression_IsNull()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();

            var dnfExpression = expressionParser.ParseExpression(null);

            var referenceBooleanExpression = new DNFExpression()
            {
            };

            dnfExpression.ShouldBeEquivalentTo(referenceBooleanExpression);
        }
    }
}
