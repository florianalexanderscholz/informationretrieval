namespace InformationRetrieval.Index
{
    public class ClusterSearchFlags : IQueryFlags
    {
        public int B1 { get; set; }
        public int B2 { get; set; }
        public int K { get; set; }
        public int R { get; set; } = 4;
        public double J { get; set; } = 0.1;
        public int L { get; set; } = 5;
    }
}