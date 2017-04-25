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
        public Token(string value, int documentPosition)
        {
            this.Value = value;
            this.Position = documentPosition;
        }

        /// <summary>
        /// Retrieves the string representation.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The position of the term in a document
        /// </summary>
        public int Position { get; set; }
    }
}