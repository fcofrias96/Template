namespace Template.SharedKernel.Domain
{
    public abstract class EntityBase : IEntityBase
    {
        public int Id { get; init; }

        public string CreatedBy { get; private set; } = AuditDefaults.SystemUser;
        public DateTimeOffset CreatedAt { get; private set; }
        public string? LastModifiedBy { get; private set; }
        public DateTimeOffset? LastModifiedAt { get; private set; }

        public bool IsDeleted { get; private set; }

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        public void MarkDeleted()
        {
            if (IsDeleted) return;
            IsDeleted = true;
        }

        void IAuditable.StampCreated(string userId, DateTimeOffset at)
        {
            CreatedBy = userId;
            CreatedAt = at;
        }

        void IAuditable.StampModified(string userId, DateTimeOffset at)
        {
            LastModifiedBy = userId;
            LastModifiedAt = at;
        }
    }
}
