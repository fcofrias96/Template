using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public readonly record struct PermissionKey(int ResourceId, int ActionId);

    public class PermissionService : BaseService<Permission>, IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly ICacheService _cache;
        private readonly TimeSpan _permissionsTtl = TimeSpan.FromDays(1);

        public PermissionService(
            ICacheService cache,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            IPermissionRepository permissionRepositoryAsync)
            : base(cache, mapper, permissionRepositoryAsync, httpContext)
        {
            _permissionRepository = permissionRepositoryAsync;
            _cache = cache;
        }

        public async Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId, CancellationToken cancellationToken)
        {
            var cacheKey = BuildPermissionsCacheKey(userId);

            // 1. Intentar leer del cache
            var permissions = _cache.Get<HashSet<PermissionKey>>(cacheKey);

            if (permissions is null)
            {
                // 2. No hay cache → ir al repo una sola vez
                var userPermissions = await _permissionRepository.GetUserPermissionsAsync(userId, cancellationToken);
                permissions = userPermissions.Select(p => new PermissionKey(p.ResourceId, p.ActionId)).ToHashSet();
                _cache.Set(cacheKey, permissions, _permissionsTtl);
            }

            // 3. Validar en memoria
            var key = new PermissionKey(resourceId, actionId);
            return permissions.Contains(key);
        }


        public async Task<HashSet<int>> GetUserPermissionIdsAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            if (user == null || !user!.Identity!.IsAuthenticated) { return new HashSet<int>(); }

            var userId = GetUserIdFromClaims(user);
            cancellationToken.ThrowIfCancellationRequested();

            var role = await _permissionRepository.GetUserPermissionIdsAsync(userId, cancellationToken);

            return role.ToHashSet();
        }

        private static string GetUserIdFromClaims(ClaimsPrincipal user)
        {
            return user.FindFirstValue("UserId") ?? string.Empty;
        }

        public static string BuildPermissionsCacheKey(string userId) => $"permissions:user{userId}";
    }
}