namespace Template.Modules.Identity.Application.Caching
{
    /// <summary>
    /// Cache keys owned by the Identity module. Anything that changes a user's roles or a role's permissions must
    /// remove the matching <see cref="UserPermissions"/> / <see cref="UserAuthData"/> entries.
    /// </summary>
    public static class IdentityCacheKeys
    {
        /// <summary>(ResourceId, ActionId) pairs granted to the user, used by permission checks.</summary>
        public static string UserPermissions(int userId) => $"identity:permissions:user:{userId}";

        /// <summary>Claims and role names of the user, used when issuing tokens.</summary>
        public static string UserAuthData(int userId) => $"identity:auth:user:{userId}";

        /// <summary>Internal user id for a public id (the JWT subject). Never changes for a user.</summary>
        public static string InternalUserId(Guid publicId) => $"identity:users:public-id:{publicId}";
    }
}
