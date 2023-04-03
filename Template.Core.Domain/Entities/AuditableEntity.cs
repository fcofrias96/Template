using Template.Core.Domain.Contracts;

namespace Template.Core.Domain.Entities
{
    public abstract class AuditableEntity<TKey> : IAuditableEntity
    {
        public abstract TKey Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        object IAuditableEntity.Id { get => Id; set => Id = (TKey)value; }
    }
}