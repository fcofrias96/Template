using Microsoft.EntityFrameworkCore;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Infrastructure;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IdentityModuleDbContext _dbContext;

        public UserRoleRepository(IdentityModuleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<int>> GetRoleIdsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateUserRolesAsync(
      int userId,
      IEnumerable<int> roleIds,
      CancellationToken cancellationToken)
        {
            // Normalizar: sin null, sin duplicados
            var desiredRoleIds = roleIds?
                .Where(r => r > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            var currentUserRoles = await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);

            // 1. Quitar los roles que ya no están en la lista
            _dbContext.UserRoles.RemoveRange(
                currentUserRoles.Where(ur => !desiredRoleIds.Contains(ur.RoleId)));

            // 2. Agregar solo los que faltan
            var currentRoleIds = currentUserRoles.Select(ur => ur.RoleId).ToHashSet();
            var newUserRoles = desiredRoleIds
                .Where(roleId => !currentRoleIds.Contains(roleId))
                .Select(roleId => new ApplicationUserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });

            await _dbContext.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);

            // 3. Un solo SaveChanges: el cambio es atómico
            await _dbContext.SaveChangesAsync(cancellationToken);
        }



        public async Task<IReadOnlyList<int>> GetUserIdsByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }

}
