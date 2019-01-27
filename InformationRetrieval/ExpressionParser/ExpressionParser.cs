/*
(c) 2019 by Florian Scholz
This file is part of InformationRetrieval.

    InformationRetrieval is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    InformationRetrieval is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with InformationRetrieval.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace InformationRetrieval.ExpressionParser
{
    /// <summary>
    /// Implements the Expression Parser
    /// </summary>
    public class ExpressionParser : IExpressionParser
    {
        /// <summary>
        /// Specifies the Or delimeter
        /// </summary>
        public char DelimiterOr { get; set; } = '|';
        
        /// <summary>
        /// Specifies the And delimeter
        /// </summary>
        public char DelimiterAnd { get; set; } = ',';
        
        /// <summary>
        /// Specifies the invert delimeter
        /// </summary>
        public char PrefixNegative { get; set; } = '!';

        /// <summary>
        /// Parses an expression and transforms the expression into a DNF tree
        /// </summary>
        /// <param name="expression">The expression string</param>
        /// <returns>A transformed DNF tree</returns>
        public DNFExpression ParseExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return new DNFExpression();
            }

            var rawDisjunctionList = expression.Split(DelimiterOr);

            DNFExpression dnfExpression = new DNFExpression();

            foreach (var rawDisjunction in rawDisjunctionList)
            {
                if (string.IsNullOrEmpty(rawDisjunction))
                {
                    continue;
                }

                Conjunction andExpression = new Conjunction();

                var conjunctionList = rawDisjunction.Split(DelimiterAnd);

                foreach (var conjunction in conjunctionList)
                {
                    if (string.IsNullOrEmpty(conjunction))
                    {
                        continue;
                    }

                    string value = conjunction.ToLower();

                    /* Adding support for negative AND */
                    bool negative = false;
                    int positionalRestrict = 0;
                    if (value[0] == PrefixNegative)
                    {
                        negative = true;
                        value = value.Substring(1);
                    }
                    else if (Char.IsDigit(value[0]))
                    {
                        var digit = (int)char.GetNumericValue(value[0]);
                        positionalRestrict = digit;
                        value = value.Substring(1);
                    }

                    Variable variableExpression = new Variable(value)
                    {
                        Negative = negative,
                        PositionalRestriction = positionalRestrict
                    };

                    andExpression.Variables.Add(variableExpression);
                }

                dnfExpression.Conjunctions.Add(andExpression);
            }

            return dnfExpression;
        }
    }
}