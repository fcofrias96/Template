namespace Template.Application.Contracts
{
    public interface IQueryParametersBase
    {
        public string? SortBy { get; set; }
        public bool Descending { get; set; }
        int Offset { get; }
        int Limit { get; }
    }
}