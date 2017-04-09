using System.Collections.Generic;

namespace InformationRetrieval.ExpressionParser
{
    /// <summary>
    /// Stores an and Expression for the DNF
    /// </summary>
    public class Conjunction
    {
        /// <summary>
        /// A list of possibly negative variables.
        /// </summary>
        public List<Variable> Variables = new List<Variable>();
    }
}
