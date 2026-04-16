using Template.Domain.Contracts;

namespace Template.Domain.Common
{
    public abstract class EntityBase : IEntityBase
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public bool IsDeleted { get; private set; }

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
        public void MarkDeleted()
        {
            if (IsDeleted) return;
            IsDeleted = true;
        }
    }
}