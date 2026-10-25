namespace Template.SharedKernel.Domain
{
    /// <summary>Entity protected by optimistic concurrency (SQL Server rowversion), also used to build ETags.</summary>
    public interface IHasRowVersion
    {
        byte[] RowVersion { get; }
    }
}
