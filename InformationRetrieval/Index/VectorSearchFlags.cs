namespace InformationRetrieval.Index
{
    public class VectorSearchFlags : IQueryFlags
    {
        public int K { get; set; } = 3;
        public int R { get; set; } = 4;
        public double J { get; set; } = 0.1;
        public int L { get; set; } = 5;
    }
}