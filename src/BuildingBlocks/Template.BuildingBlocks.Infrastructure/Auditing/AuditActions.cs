namespace Template.BuildingBlocks.Infrastructure.Auditing
{
    /// <summary>
    /// Values stored in <see cref="Audit.Action"/>. Kept identical to the Identity
    /// permission-action ids they were originally taken from so existing audit rows stay readable.
    /// </summary>
    public static class AuditActions
    {
        public const int Create = 1;
        public const int Update = 3;
        public const int Delete = 4;
    }
}
