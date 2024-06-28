namespace Template.Domain.Contracts
{
    public interface IEntityBase
    {
        object Id { get; set; }
        string CreatedBy { get; set; }
        DateTime Created { get; set; }
        string? LastModifiedBy { get; set; }
        DateTime? LastModified { get; set; }
        bool IsDeleted { get; set; }
    }
}