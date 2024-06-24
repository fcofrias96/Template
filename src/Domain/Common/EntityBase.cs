using Template.Domain.Contracts;

namespace Template.Domain.Common
{
    public abstract class EntityBase<TKey> : IEntityBase
    {
        public abstract TKey Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public bool Active { get; set; }

        object IEntityBase.Id { get => Id; set => Id = (TKey)value; }
    }
}