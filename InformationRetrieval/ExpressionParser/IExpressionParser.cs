namespace InformationRetrieval.ExpressionParser
{
    public interface IExpressionParser
    {
        DNFExpression ParseExpression(string expression);
    }
}