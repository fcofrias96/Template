using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class PermissionRepositoryAsync : BaseRepositoryAsync<Permission, SecurityDbContext>, IPermissionRepositoryAsync
    {
        private readonly SecurityDbContext _dbContext;

        public PermissionRepositoryAsync(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
            _dbContext = securityDb;
        }

        public async Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId)
        {
            var hasPermission = await _dbContext.RolePermissions
                .AnyAsync(rp =>
                    rp.Role.UserRoles.Any(ur => ur.UserId == userId) &&
                    rp.Permission.Resource.Id == resourceId &&
                    rp.Permission.ActionType.Id == actionId
                );

            return hasPermission;
        }
    }
}