using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval.ExpressionParser
{
    /// <summary>
    /// Stores an and Expression for the DNF
    /// </summary>
    public class AndExpression
    {
        /// <summary>
        /// A list of possibly negative variables.
        /// </summary>
        public List<Variable> Variables = new List<Variable>();
    }
}
