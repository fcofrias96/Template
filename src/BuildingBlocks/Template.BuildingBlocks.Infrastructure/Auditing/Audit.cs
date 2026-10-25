namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    public class Audit
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }
    }
}
