using Microsoft.AspNetCore.Http;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Caching;
using Template.Modules.Identity.Application.Dtos.Roles;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Application.Services.Base;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Dtos.RolePermission;
using Template.Modules.Identity.Domain.Entities;
using Template.SharedKernel.Results;

namespace Template.Modules.Identity.Application.Services
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
            ICacheService cache,
            IHttpContextAccessor httpContext)
            : base(repositoryAsync, cache, httpContext)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _userRoleRepository = userRoleRepository;
            _cache = cache;
        }

        public override Task<Result<TDestination>> GetByIdProjectedAsync<TDestination>(
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


        public override async Task<Result> PatchAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
        {
            // Validate and update the role first, so a rejected request never touches its permissions.
            var result = await base.PatchAsync(id, source, cancellationToken);
            if (result.IsFailure)
            {
                return result;
            }

            if (source is RoleUpdateDto roleUpdateDTO)
            {
                // 1. Actualizar permisos del rol
                await _rolePermissionRepository.UpdateAsync(id, roleUpdateDTO.Permissions, cancellationToken);

                // 2. Invalidar cache de todos los usuarios que tienen este rol
                await InvalidatePermissionsCacheForRoleAsync(id, cancellationToken);
            }

            return Result.Success();
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
                _cache.Remove(IdentityCacheKeys.UserPermissions(userId));
            }
        }
    }

}
