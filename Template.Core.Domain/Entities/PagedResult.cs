namespace Template.Core.Domain.Entities
{
    public class PagedResult<T>
    {
        public string Href { get; set; }
        public string? Next { get; set; }
        public string? Prev { get; set; }
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<T> Items { get; set; }
    }
}