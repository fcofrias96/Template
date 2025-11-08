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



        public async Task<RolePermission[]> GetAllAsync(string roleId, int[] permissionsId, CancellationToken cancellationToken)
        {
            return await _dbContext.RolePermissions.Where(x => x.RoleId == roleId && permissionsId.Contains(x.PermissionId)).ToArrayAsync();
        }



        public async Task UpdateAsync(string roleId, int[] newPermissionIds, CancellationToken cancellationToken)
        {
            var newPermissionSet = new HashSet<int>(newPermissionIds);
            await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId && !newPermissionSet.Contains(rp.PermissionId))
                .ExecuteDeleteAsync(cancellationToken);

            var currentPermissionIds = await _dbContext.RolePermissions
                .Where(pr => pr.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);

            var currentPermissionSet = currentPermissionIds.ToHashSet();

            var toAdd = newPermissionSet
                .Where(permissionId => !currentPermissionSet.Contains(permissionId))
                .Select(permissionId => new RolePermission
                {
                    PermissionId = permissionId,
                    RoleId = roleId
                })
            .ToList();

            if (toAdd.Count > 0)
            {
                await _dbContext.RolePermissions.AddRangeAsync(toAdd, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}