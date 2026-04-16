namespace Template.Application.Caching
{
    // =========================
    // Namespaces
    // =========================

    public static class SecurityCacheNamespaces
    {
        public const string MenuGlobal = "Security:Menu:ns:v";
        public const string PermissionsGlobal = "Security:Permissions:ns:v";

        // Datos derivados del Owner (BE + Subscription)
        public const string OwnerProfileGlobal = "Security:OwnerProfile:ns:v";
        public static string OwnerProfileByUser(int userId) => $"Security:OwnerProfile:user:{userId}:ns:v";

        // Auth cache
        public const string AuthGlobal = "Security:Auth:ns:v";
        public static string AuthByUser(int userId) => $"Security:Auth:user:{userId}:ns:v";

        // Roles/claims globales (opcional)
        public const string RolesGlobal = "Security:Roles:ns:v";

        public static string PermissionsByUser(int userId) => $"Security:Permissions:user:{userId}:ns:v";
    }
}