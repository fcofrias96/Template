using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Infrastructure;
using Template.Modules.Identity.Application.Dtos.UserPermission;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Repositories
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        private readonly IdentityModuleDbContext _dbContext;

        public PermissionRepository(IdentityModuleDbContext securityDb) : base(securityDb)
        {
            _dbContext = securityDb;
        }

        public async Task<IReadOnlyList<int>> GetUserPermissionIdsAsync(int userId, CancellationToken cancellationToken)
        { 
            return await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken)
        {
            return await _dbContext.RolePermissions
                .Where(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId))
                .Select(rp => new UserPermissionDto
                {
                    ResourceId = rp.Permission.ResourceId,
                    ActionId = rp.Permission.ActionId
                })
                .ToListAsync();
        }

        
    }
}
