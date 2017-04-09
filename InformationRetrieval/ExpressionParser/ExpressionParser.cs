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
                return new DNFExpression();
            }
        }
    }
}