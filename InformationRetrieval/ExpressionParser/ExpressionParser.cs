namespace InformationRetrieval.ExpressionParser
{
    public class ExpressionParser : IExpressionParser
    {
        public char DelimiterOr { get; set; } = '|';
        public char DelimiterAnd { get; set; } = ',';
        public char PrefixNegative { get; set; } = '!';

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

                    string value = conjunction;

                    /* Adding support for negative AND */
                    bool negative = false;
                    if (value[0] == PrefixNegative)
                    {
                        negative = true;
                        value = value.Substring(1);
                    }

                    Variable variableExpression = new Variable(value)
                    {
                        Negative = negative
                    };

                    andExpression.Variables.Add(variableExpression);
                }

                dnfExpression.Conjunctions.Add(andExpression);
            }

            return dnfExpression;
        }
    }
}