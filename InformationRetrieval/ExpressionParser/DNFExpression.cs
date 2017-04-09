using System.Collections.Generic;

namespace InformationRetrieval.ExpressionParser
{
    /// <summary>
    /// Provides a DTO representation of a boolean expression in DNF
    /// </summary>
    public class DNFExpression
    {
        /// <summary>
        /// Stores the AND expressions.
        /// </summary>
        public List<AndExpression> SubExpressions { get; set; } = new List<AndExpression>();
    }
}