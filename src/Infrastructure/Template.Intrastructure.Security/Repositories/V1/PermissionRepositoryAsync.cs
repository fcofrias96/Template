using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Dtos.UserPermission;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class PermissionRepositoryAsync : BaseRepositoryAsync<Permission>, IPermissionRepositoryAsync
    {
        private readonly SecurityDbContext _dbContext;

        public PermissionRepositoryAsync(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
            _dbContext = securityDb;
        }

        public async Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(string userId)
        {
            return await _dbContext.RolePermissions
                .Where(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId))
                .Select(rp => new UserPermissionDto
                {
                    ResourceId = rp.Permission.Resource.Id,
                    ActionId = rp.Permission.ActionType.Id
                })
                .ToListAsync();
        }
    }
}