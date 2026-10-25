namespace Template.SharedKernel.Domain
{
    /// <summary>
    /// Entity whose creation/modification is stamped by the persistence layer.
    /// The stamps are read-only on the entity; only the infrastructure calls <see cref="StampCreated"/> and
    /// <see cref="StampModified"/>, which entities implement explicitly so they do not show up on the entity type.
    /// </summary>
    public interface IAuditable
    {
        string CreatedBy { get; }
        DateTimeOffset CreatedAt { get; }
        string? LastModifiedBy { get; }
        DateTimeOffset? LastModifiedAt { get; }

        void StampCreated(string userId, DateTimeOffset at);
        void StampModified(string userId, DateTimeOffset at);
    }
}
