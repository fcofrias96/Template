namespace Template.BuildingBlocks.Application.Models.Wrappers
{
    public class PagedCollection<T>
    {
        public string HRef { get; set; }
        public string? Next { get; set; }
        public string? Prev { get; set; }
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public IReadOnlyList<T> Elements { get; set; }
    }
}
