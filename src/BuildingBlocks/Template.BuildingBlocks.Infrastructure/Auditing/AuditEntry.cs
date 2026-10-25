namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    public class AuditEntry
    {
        public string UserName { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object?> KeyValues { get; } = new Dictionary<string, object?>();
        public Dictionary<string, object?> OldValues { get; } = new Dictionary<string, object?>();
        public Dictionary<string, object?> NewValues { get; } = new Dictionary<string, object?>();
        public int Action { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
    }
}
