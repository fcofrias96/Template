using Template.Domain.Contracts;

namespace Template.Domain.Common
{
    public abstract class EntityBase<TKey> : IEntityBase
    {
        public abstract TKey Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        object IEntityBase.Id { get => Id; set => Id = (TKey)value; }
    }
}