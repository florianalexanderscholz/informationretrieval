namespace InformationRetrieval.Index
{
    public interface IQueryFlags
    {
        int R { get; set; }
        double J { get; set; }
        int L { get; set; }
    }
}