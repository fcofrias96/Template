using Microsoft.EntityFrameworkCore;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class RolePermissionRepositoryAsync : IRolePermissionRepositoryAsync
    {
        private readonly SecurityDbContext _dbContext;

        public RolePermissionRepositoryAsync(SecurityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRanges(RolePermission[] rolePermissions)
        {
            await _dbContext.RolePermissions.AddRangeAsync(rolePermissions);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RolePermission[]> GetAlls(string roleId, int[] permissionsId)
        {
            return await _dbContext.RolePermissions.Where(x => x.RoleId == roleId && permissionsId.Contains(x.ResourceActionId)).ToArrayAsync();
        }

        public async Task RemoveRanges(RolePermission[] rolePermissions)
        {
            _dbContext.RolePermissions.RemoveRange(rolePermissions);
            await _dbContext.SaveChangesAsync();
        }
    }
}