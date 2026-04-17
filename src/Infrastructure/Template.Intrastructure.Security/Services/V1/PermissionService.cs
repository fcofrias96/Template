using System.Security.Claims;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;
using Template.Security.Repositories.V1;

namespace Template.Security.Services.V1
{
    public readonly record struct PermissionKey(int ResourceId, int ActionId);

    public class PermissionService : BaseService<Permission>, IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cache;
        private readonly TimeSpan _permissionsTtl = TimeSpan.FromDays(1);

        public PermissionService(
            IPermissionRepository permissionRepositoryAsync,
            IUserRepository userRepositoryAsync,
            BaseServiceDeps<Permission> deps)
            : base(deps)
        {
            _permissionRepository = permissionRepositoryAsync;
            _cache = deps.Cache;
            _userRepository = userRepositoryAsync;
        }

        public async Task<bool> HasPermissionAsync(string userPublicId, int resourceId, int actionId, CancellationToken cancellationToken)
        {
            var userId = await GetInternalUserIdAsync(userPublicId, cancellationToken);
            if(userId is null) return false;

            var cacheKey = BuildPermissionsCacheKey(userId.Value);

            // 1. Intentar leer del cache
            var permissions = _cache.Get<HashSet<PermissionKey>>(cacheKey);

            if (permissions is null)
            {
                // 2. No hay cache → ir al repo una sola vez
                var userPermissions = await _permissionRepository.GetUserPermissionsAsync(userId.Value, cancellationToken);
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

        private static int GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue("UserId");
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private async Task<int?> GetInternalUserIdAsync(string userPublicId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userPublicId, out var publicId))
            {
                return null;
            }

            var cacheKey = BuildInternalIdByPublicIdKey(publicId);

            var cachedUserId = _cache.Get<int?>(cacheKey);
            if (cachedUserId.HasValue)
            {
                return cachedUserId.Value;
            }

            var userId = await _userRepository.GetIdByPublicIdAsync(publicId, cancellationToken);

            if (userId.HasValue)
            {
                _cache.Set(cacheKey, userId.Value, TimeSpan.FromDays(30));
            }

            return userId;
        }


        public static string BuildPermissionsCacheKey(int userId) => $"permissions:user{userId}";

        public static string BuildInternalIdByPublicIdKey(Guid publicId) => $"users:public-id:{publicId}:internal-id";
    }
}