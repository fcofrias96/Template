namespace Template.Application.Caching.Contexts
{
    // =========================
    // Cache key contexts
    // =========================

    public readonly record struct MenuCacheContext(
        string PermissionSignatureHash,
        int OwnerEstablishmentId);

    public readonly record struct EntityListAllContext(
        string EntityName);

    public readonly record struct EntityListByScopeContext(
        string EntityName,
        string ScopeName,
        int ScopeId);

    public readonly record struct OwnerBusinessEstablishmentContext(
        int UserId);

    public readonly record struct UserAuthCacheContext(
        int UserId);

    public readonly record struct UserPermissionsContext(
        int UserId);

    public readonly record struct EntityPageContext(
        string EntityName,
        string ScopeName,
        int? ScopeId,
        int Skip,
        int Take,
        string? Sort,
        string? FilterSignatureHash);
}
