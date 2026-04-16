using Microsoft.EntityFrameworkCore;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly SecurityDbContext _dbContext;

        public UserRoleRepository(SecurityDbContext dbContext)
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

            // 1. Eliminar todo lo que el usuario tiene actualmente
            await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

            // 2. Si no hay roles nuevos, terminamos
            if (desiredRoleIds.Count == 0)
                return;

            // 3. Insertar en bloque los roles nuevos
            var newUserRoles = desiredRoleIds
                .Select(roleId => new ApplicationUserRole
                {
                    UserId = userId,
                    RoleId = roleId
                })
                .ToList();

            await _dbContext.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);
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