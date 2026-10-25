namespace Template.SharedKernel.Domain
{
    /// <summary>Entity that is flagged as deleted instead of being removed; hidden by a global query filter.</summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }

        void MarkDeleted();
    }
}
