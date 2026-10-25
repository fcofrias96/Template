namespace Template.SharedKernel.Domain
{
    /// <summary>
    /// Composite of every entity role. Required by the generic BaseRepository/BaseService, which rely on all of them
    /// (soft delete, ETags from RowVersion/LastModifiedAt). Entities outside that pipeline can implement only the
    /// roles they need.
    /// </summary>
    public interface IEntityBase : IEntity, IAuditable, ISoftDeletable, IHasRowVersion
    {
    }
}
