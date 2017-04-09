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
        public List<Conjunction> Conjunctions { get; set; } = new List<Conjunction>();
    }
}