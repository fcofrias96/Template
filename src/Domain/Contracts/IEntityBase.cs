namespace Template.Domain.Contracts
{
    public interface IEntityBase
    {
        int Id { get; set; }
        string CreatedBy { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        string? LastModifiedBy { get; set; }
        DateTimeOffset? LastModifiedAt { get; set; }
        bool IsDeleted { get; }
        byte[] RowVersion { get; }
        void MarkDeleted();
    }
}