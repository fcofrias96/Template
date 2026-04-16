#nullable enable
using Template.Application.Caching.Contexts;
using Template.Application.Constants;
using Template.Application.Contracts.Caching;

namespace Template.Application.Caching
{
    /// <summary>
    /// ✅ Registry:
    /// - Guarda policies en object, pero devuelve con validación fuerte
    /// - Mensajes de error más claros
    /// </summary>
    public sealed class CachePolicyRegistry : ICachePolicyRegistry
    {
        private readonly Dictionary<string, object> _policies = new(StringComparer.Ordinal);

        public CachePolicyRegistry()
        {
            // ✅ security.menu
            _policies[CachePolicyNames.SecurityMenu] = new CacheKeyDefinition<MenuCacheContext>
            {
                Parts = ctx => new object?[]
                {
                "Security", "Menu",
                "p", ctx.PermissionSignatureHash,
                "be", ctx.OwnerEstablishmentId
                },
                NamespaceKeys = new Func<MenuCacheContext, string>[]
                {
                    _ => SecurityCacheNamespaces.MenuGlobal,
                    _ => SecurityCacheNamespaces.PermissionsGlobal
                },
                DataTtl = TimeSpan.FromHours(12),
                NamespaceTtl = TimeSpan.FromDays(7)
            };

            // ✅ patrón: lista completa
            _policies[CachePolicyNames.EntityListAll] = new CacheKeyDefinition<EntityListAllContext>
            {
                Parts = ctx => new object?[] { ctx.EntityName, "list", "all" },
                NamespaceKeys = new Func<EntityListAllContext, string>[]
                {
                    ctx => $"{ctx.EntityName}:ns:v"
                },
                DataTtl = TimeSpan.FromDays(1),
                NamespaceTtl = TimeSpan.FromDays(7)
            };

            // ✅ lista por scope
            _policies[CachePolicyNames.EntityListByScope] = new CacheKeyDefinition<EntityListByScopeContext>
            {
                Parts = ctx => new object?[] { ctx.EntityName, "by", ctx.ScopeName, ctx.ScopeId, "list" },
                NamespaceKeys = new Func<EntityListByScopeContext, string>[]
                {
                    ctx => $"{ctx.EntityName}:ns:v",
                    ctx => $"{ctx.EntityName}:by:{ctx.ScopeName}:{ctx.ScopeId}:ns:v"
                },
                DataTtl = TimeSpan.FromDays(1),
                NamespaceTtl = TimeSpan.FromDays(7)
            };

            // ✅ auth cache (claims/roles/etc.)
            _policies[CachePolicyNames.SecurityAuthUserAuthData] = new CacheKeyDefinition<UserAuthCacheContext>
            {
                Parts = ctx => new object?[]
                {
                    "Security", "Auth", "UserAuthData",
                    "user", ctx.UserId
                },
                NamespaceKeys = new Func<UserAuthCacheContext, string>[]
                {
                    _ => SecurityCacheNamespaces.AuthGlobal,
                    ctx => SecurityCacheNamespaces.AuthByUser(ctx.UserId),

                    // Opcional: inclúyelos solo si auth depende de cambios globales del sistema
                    _ => SecurityCacheNamespaces.RolesGlobal,
                    _ => SecurityCacheNamespaces.PermissionsGlobal
                },
                DataTtl = TimeSpan.FromHours(1),
                NamespaceTtl = TimeSpan.FromDays(7)
            };

            // ✅ permisos (keys)
            _policies[CachePolicyNames.SecurityPermissionsUserPermissionKeys] = new CacheKeyDefinition<UserPermissionsContext>
            {
                Parts = ctx => new object?[]
                {
                    "Security", "Permissions", "Keys",
                    "user", ctx.UserId
                },
                NamespaceKeys = new Func<UserPermissionsContext, string>[]
                {
                    _ => SecurityCacheNamespaces.PermissionsGlobal,
                    ctx => SecurityCacheNamespaces.PermissionsByUser(ctx.UserId)
                },
                DataTtl = TimeSpan.FromDays(1),
                NamespaceTtl = TimeSpan.FromDays(7)
            };

            // ✅ permisos (ids)
            _policies[CachePolicyNames.SecurityPermissionsUserPermissionIds] = new CacheKeyDefinition<UserPermissionsContext>
            {
                Parts = ctx => new object?[]
                {
                    "Security", "Permissions", "Ids",
                    "user", ctx.UserId
                },
                NamespaceKeys = new Func<UserPermissionsContext, string>[]
                {
                    _ => SecurityCacheNamespaces.PermissionsGlobal,
                    ctx => SecurityCacheNamespaces.PermissionsByUser(ctx.UserId)
                },
                DataTtl = TimeSpan.FromDays(1),
                NamespaceTtl = TimeSpan.FromDays(7)
            };

            // ✅ paginación por scope (TTL corto)
            _policies[CachePolicyNames.EntityPageByScope] = new CacheKeyDefinition<EntityPageContext>
            {
                Parts = ctx => new object?[]
                {
                    ctx.EntityName,
                    "page",
                    "scope", ctx.ScopeName, ctx.ScopeId.HasValue ? ctx.ScopeId : "none",
                    "skip", ctx.Skip,
                    "take", ctx.Take,
                    "sort", ctx.Sort ?? "none",
                    // ✅ hash para evitar keys gigantes
                    "filter", ctx.FilterSignatureHash ?? "none"
                },
                NamespaceKeys = new Func<EntityPageContext, string>[]
                {
                    ctx => $"{ctx.EntityName}:ns:v",
                    ctx => $"{ctx.EntityName}:by:{ctx.ScopeName}:{ctx.ScopeId}:ns:v"
                },
                DataTtl = TimeSpan.FromMinutes(5),
                NamespaceTtl = TimeSpan.FromDays(7)
            };
        }

        public CacheKeyDefinition<TContext> Get<TContext>(string name)
        {
            if (!_policies.TryGetValue(name, out var def))
                throw new InvalidOperationException($"Cache policy '{name}' not registered.");

            if (def is not CacheKeyDefinition<TContext> typed)
            {
                // Mejor mensaje cuando se pide con TContext equivocado
                var registeredFor = def.GetType().IsGenericType
                    ? def.GetType().GenericTypeArguments.FirstOrDefault()?.Name
                    : def.GetType().Name;

                throw new InvalidOperationException(
                    $"Cache policy '{name}' is registered for '{registeredFor}', but was requested as '{typeof(TContext).Name}'.");
            }

            return typed;
        }
    }
}