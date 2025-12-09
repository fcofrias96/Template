using Microsoft.EntityFrameworkCore;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class UserRoleRepositoryAsync : IUserRoleRepositoryAsync
    {
        private readonly SecurityDbContext _dbContext;

        public UserRoleRepositoryAsync(SecurityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<string>> GetRoleIdsByUserIdAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateUserRolesAsync(
      string userId,
      IEnumerable<string> roleIds,
      CancellationToken cancellationToken)
        {
            // Normalizar: sin null, sin duplicados
            var desiredRoleIds = roleIds?
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct()
                .ToList() ?? new List<string>();

            // 1. Eliminar todo lo que el usuario tiene actualmente
            await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

            // 2. Si no hay roles nuevos, terminamos
            if (desiredRoleIds.Count == 0)
                return;

            // 3. Insertar en bloque los roles nuevos
            var newUserRoles = desiredRoleIds
                .Select(roleId => new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                })
                .ToList();

            await _dbContext.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }



        public async Task<IReadOnlyList<string>> GetUserIdsByRoleIdAsync(
            string roleId,
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