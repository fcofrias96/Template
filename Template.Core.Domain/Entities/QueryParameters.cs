namespace Template.Core.Domain.Entities
{
    public class QueryParameters<TEntity>
    {
        public bool Descending { get; set; }
        public int Limit { get; set; } = 15;
        public int Offset { get; set; }
        public string Filter { get; set; }
        public List<string> SortableProperties { get; set; } = new List<string>();

    }
}
