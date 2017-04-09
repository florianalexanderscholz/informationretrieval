namespace InformationRetrieval.ExpressionParser
{
    public class ExpressionParser : IExpressionParser
    {
        public DNFExpression ParseExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return new DNFExpression();
            }
            else
            {
                var andExpressions = expression.Split('|');

                DNFExpression dnfExpression = new DNFExpression();

                foreach (var subExpression in andExpressions)
                {
                    if (string.IsNullOrEmpty(subExpression))
                    {
                        continue;
                    }

                    AndExpression andExpression = new AndExpression();

                    var variablesExpression = subExpression.Split(',');

                    foreach (var variable in variablesExpression)
                    {
                        if (string.IsNullOrEmpty(variable))
                        {
                            continue;
                        }

                        string value = variable;

                        bool negative = false;
                        if (value[0] == '!')
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

                    dnfExpression.SubExpressions.Add(andExpression);
                }

                return dnfExpression;
            }
        }
    }
}