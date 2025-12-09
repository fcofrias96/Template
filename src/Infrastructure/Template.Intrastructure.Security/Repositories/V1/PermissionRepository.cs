using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Dtos.UserPermission;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class PermissionRepository : BaseRepositoryAsync<Permission>, IPermissionRepository
    {
        private readonly SecurityDbContext _dbContext;

        public PermissionRepository(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
            _dbContext = securityDb;
        }

        public async Task<IReadOnlyList<int>> GetUserPermissionIdsAsync(string userId, CancellationToken cancellationToken)
        { 
            return await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
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