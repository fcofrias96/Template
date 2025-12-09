using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
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

    public class RoleService : BaseServiceAsync<Role>, IRoleService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICacheService _cache;

        public RoleService(
            ICacheService cache,
            IMapper mapper,
            IRoleRepository repositoryAsync,
            IHttpContextAccessor httpContext,
            IRolePermissionRepository rolePermissionRepository,
            IUserRoleRepository userRoleRepository)
            : base(cache, mapper, repositoryAsync, httpContext)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _userRoleRepository = userRoleRepository;
            _cache = cache;
        }

        public override Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(
            TKey id,
            CancellationToken cancellationToken)
        {
            string idString = id?.ToString() ?? string.Empty;
            return base.GetByIdProjectedAsync<string, TDestination>(idString, cancellationToken);
        }

        public  async Task<ApiResponse<IReadOnlyList<RolePermissionDto>>> GetPermissionsAsync(string roleId, CancellationToken cancellationToken)
        {
            var permissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(roleId, cancellationToken);

            return new ApiResponse<IReadOnlyList<RolePermissionDto>>(permissions);
        }


        public override async Task UpdateAsync<TKey, TSource>(
            TKey id,
            TSource source,
            CancellationToken cancellationToken)
        {
            if (source is RoleUpdateDto roleUpdateDTO && id is string roleId)
            {
                // 1. Actualizar permisos del rol
                await _rolePermissionRepository.UpdateAsync(roleId, roleUpdateDTO.Permissions, cancellationToken);

                // 2. Invalidar cache de todos los usuarios que tienen este rol
                await InvalidatePermissionsCacheForRoleAsync(roleId, cancellationToken);
            }

            await base.UpdateAsync(id, source, cancellationToken);
        }

        public async Task UpdatePermissionsAsync(string roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken)
        {
            var idsArray = permissionIds?
                .Distinct()
                .ToArray() ?? Array.Empty<int>();

            await _rolePermissionRepository.UpdateAsync(roleId, idsArray, cancellationToken);

            await InvalidatePermissionsCacheForRoleAsync(roleId, cancellationToken);
        }


        private async Task InvalidatePermissionsCacheForRoleAsync(
            string roleId,
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