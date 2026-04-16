using Template.Application.Contracts.Services.V1;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.Wrappers;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.RolePermission;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{

    public class RoleService : BaseService<ApplicationRole>, IRoleService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICacheService _cache;

        public RoleService(
            IRoleRepository repositoryAsync,
            IRolePermissionRepository rolePermissionRepository,
            IUserRoleRepository userRoleRepository,
            BaseServiceDeps<ApplicationRole> baseServiceDeps)
            : base(baseServiceDeps)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _userRoleRepository = userRoleRepository;
            _cache = baseServiceDeps.Cache;
        }

        public override Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TDestination>(
            int id,
            CancellationToken cancellationToken)
        {
            return base.GetByIdProjectedAsync<TDestination>(id, cancellationToken);
        }

        public  async Task<ApiResponse<IReadOnlyList<RolePermissionDto>>> GetPermissionsAsync(int roleId, CancellationToken cancellationToken)
        {
            var permissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(roleId, cancellationToken);

            return new ApiResponse<IReadOnlyList<RolePermissionDto>>(permissions);
        }


        public override async Task UpdateAsync< TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
        {
            if (source is RoleUpdateDto roleUpdateDTO && id is int roleId)
            {
                // 1. Actualizar permisos del rol
                await _rolePermissionRepository.UpdateAsync(roleId, roleUpdateDTO.Permissions, cancellationToken);

                // 2. Invalidar cache de todos los usuarios que tienen este rol
                await InvalidatePermissionsCacheForRoleAsync(roleId, cancellationToken);
            }

            await base.UpdateAsync(id, source, cancellationToken);
        }

        public async Task UpdatePermissionsAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken)
        {
            var idsArray = permissionIds?
                .Distinct()
                .ToArray() ?? Array.Empty<int>();

            await _rolePermissionRepository.UpdateAsync(roleId, idsArray, cancellationToken);

            await InvalidatePermissionsCacheForRoleAsync(roleId, cancellationToken);
        }


        private async Task InvalidatePermissionsCacheForRoleAsync(
            int roleId,
            CancellationToken cancellationToken)
        {
            var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(roleId, cancellationToken);

            foreach (var userId in userIds)
            {
                _cache.Remove(PermissionService.BuildPermissionsCacheKey(userId));
            }
        }
    }

}