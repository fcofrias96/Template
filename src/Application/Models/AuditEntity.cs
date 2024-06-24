using Actions = Template.Application.Enums.Actions;

namespace Template.Application.Models
{
    public class AuditEntry
    {
        public string UserName { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object?> KeyValues { get; } = new Dictionary<string, object?>();
        public Dictionary<string, object?> OldValues { get; } = new Dictionary<string, object?>();
        public Dictionary<string, object?> NewValues { get; } = new Dictionary<string, object?>();
        public Actions Action { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
    }
}