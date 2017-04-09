using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using InformationRetrieval.ExpressionParser;
using NSubstitute;
using Xunit;

namespace UnitTests.ExpressionParser
{
    public class ExpressionParser_ParseExpression
    {
        [Fact]
        public void ExpressionParser_ParseExpression_ParseOneElement()
        {
            var expressionParser = new InformationRetrieval.ExpressionParser.ExpressionParser();

            var dnfExpression = expressionParser.ParseExpression("(Schiff,Fahrzeug)");

            var referenceBooleanExpression = new DNFExpression()
            {
                SubExpressions = new List<AndExpression>()
                {
                    new AndExpression()
                    {
                        Variables = new List<Variable>()
                        {
                            new Variable("Schiff"),
                            new Variable("Fahrzeug")
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
