using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class PermissionRepositoryAsync : RepositoryBaseAsync<Permission, SecurityDbContext>, IPermissionRepositoryAsync
    {
        private readonly SecurityDbContext _dbContext;

        public PermissionRepositoryAsync(
                    ICreateRepositoryAsync<Permission, SecurityDbContext> createRepository,
                    IUpdateRepositoryAsync<Permission, SecurityDbContext> updateRepository,
                    IDeleteRepositoryAsync<Permission, SecurityDbContext> deleteRepository,
                    IReadRepositoryAsync<Permission, SecurityDbContext> readRepository,
                    SecurityDbContext dbContext

                    ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
            _dbContext = dbContext;
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