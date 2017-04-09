namespace InformationRetrieval.ExpressionParser
{
    /// <summary>
    /// DTO for boolean variables
    /// </summary>
    public class Variable
    { 
        public Variable(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The variable as a string
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if a variable is negative.
        /// </summary>
        public bool Negative { get; set; } = false;
    }
}