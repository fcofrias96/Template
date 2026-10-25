namespace Template.SharedKernel.Domain
{
    public static class AuditDefaults
    {
        /// <summary>
        /// Author recorded for rows not created by an authenticated user (seed data, system processes).
        /// Also used as the database default of the CreatedBy column.
        /// </summary>
        public const string SystemUser = "System";
    }
}
