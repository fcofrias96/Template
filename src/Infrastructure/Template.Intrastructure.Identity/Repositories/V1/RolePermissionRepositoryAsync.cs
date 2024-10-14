using Microsoft.EntityFrameworkCore;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContext.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class RolePermissionRepositoryAsync : IRolePermissionRepositoryAsync
    {
        private readonly IdentityContext _dbContext;

        public RolePermissionRepositoryAsync(IdentityContext dbContext)
        {
            _dbContext = dbContext;
        }

        public  async Task AddRanges(RolePermission[] rolePermissions)
        {
           await _dbContext.RolePermissions.AddRangeAsync(rolePermissions);
           await _dbContext.SaveChangesAsync();
        }

        public async Task<RolePermission[]> GetAlls(string roleId, int[] permissionsId)
        {
            return await _dbContext.RolePermissions.Where(x => x.RoleId == roleId && permissionsId.Contains(x.PermissionId)).ToArrayAsync();
        }

        public async Task RemoveRanges(RolePermission[] rolePermissions)
        {
             _dbContext.RolePermissions.RemoveRange(rolePermissions);
            await _dbContext.SaveChangesAsync();
        }
    }
}