using Newtonsoft.Json;
using Template.BuildingBlocks.Infrastructure.Auditing;

namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    public static class AuditExtension
    {
        public static Audit ToAudit(this AuditEntry entry)
        {
            return new Audit
            {
                UserName = entry.UserName,
                Action = entry.Action.ToString(),
                TableName = entry.TableName,
                DateTimeOffset = DateTimeOffset.UtcNow,
                PrimaryKey = JsonConvert.SerializeObject(entry.KeyValues),
                OldValues = entry.OldValues.Count == 0 ? null : JsonConvert.SerializeObject(entry.OldValues),
                NewValues = entry.NewValues.Count == 0 ? null : JsonConvert.SerializeObject(entry.NewValues),
                AffectedColumns = entry.ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(entry.ChangedColumns)
            };
        }
    }
}
