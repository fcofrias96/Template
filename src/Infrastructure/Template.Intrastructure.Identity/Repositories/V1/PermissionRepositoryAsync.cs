using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class PermissionRepositoryAsync : RepositoryBaseAsync<Permission>, IPermissionRepositoryAsync
    {
        private readonly IdentityContext _dbContext;

        public PermissionRepositoryAsync(
                    ICreateRepositoryAsync<Permission> createRepository,
                    IUpdateRepositoryAsync<Permission> updateRepository,
                    IDeleteRepositoryAsync<Permission> deleteRepository,
                    IReadRepositoryAsync<Permission> readRepository,
                    IdentityContext dbContext

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
                    rp.Permission.Action.Id == actionId
                );

            return hasPermission;
        }
    }
}