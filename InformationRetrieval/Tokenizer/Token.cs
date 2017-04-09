namespace InformationRetrieval.Tokenizer
{
    /// <summary>
    /// Token DTO
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">A value string</param>
        public Token(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Retrieves the string representation.
        /// </summary>
        public string Value { get; set; }
    }
}