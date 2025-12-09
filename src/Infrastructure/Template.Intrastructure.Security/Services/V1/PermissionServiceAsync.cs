using AutoMapper;
using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public readonly record struct PermissionKey(int ResourceId, int ActionId);

    public class PermissionServiceAsync : BaseServiceAsync<Permission>, IPermissionServiceAsync
        {
            private readonly IPermissionRepositoryAsync _permissionRepository;
            private readonly ICacheService _cache;
            private readonly TimeSpan _permissionsTtl = TimeSpan.FromDays(1);

            public PermissionServiceAsync(
                ICacheService cache,
                IMapper mapper,
                IHttpContextAccessor httpContext,
                IPermissionRepositoryAsync permissionRepositoryAsync)
                : base(cache, mapper, permissionRepositoryAsync, httpContext)
            {
                _permissionRepository = permissionRepositoryAsync;
                _cache = cache;
            }

            public async Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId)
            {
                var cacheKey = BuildPermissionsCacheKey(userId);

                // 1. Intentar leer del cache
                var permissions = _cache.Get<HashSet<PermissionKey>>(cacheKey);

                if (permissions is null)
                {
                    // 2. No hay cache → ir al repo una sola vez
                    var userPermissions = await _permissionRepository.GetUserPermissionsAsync(userId);

                    permissions = new HashSet<PermissionKey>(
                        userPermissions.Select(p => new PermissionKey(p.ResourceId, p.ActionId))
                    );

                    _cache.Set(cacheKey, permissions, _permissionsTtl);
                }

                // 3. Validar en memoria
                var key = new PermissionKey(resourceId, actionId);
                return permissions.Contains(key);
            }

            public static string BuildPermissionsCacheKey(string userId) => $"permissions:{userId}";
        } 
}