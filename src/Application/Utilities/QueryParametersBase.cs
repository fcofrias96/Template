using Template.Application.Contracts;

namespace Template.Application.Utilities
{
    public abstract class QueryParametersBase : IQueryParametersBase
    {
        public abstract string? SortBy { get; set; }
        public string? Filter { get; set; }
        public bool Descending { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; } = 30;
    }
}