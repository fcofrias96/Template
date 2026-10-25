using Microsoft.EntityFrameworkCore;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Infrastructure;
using Template.Modules.Identity.Application.Dtos.RolePermission;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Repositories
{
    public class RolePermissionRepository :  IRolePermissionRepository
    {
        private readonly IdentityModuleDbContext _dbContext;

        public RolePermissionRepository(IdentityModuleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Agrega permisos al rol sin tocar los existentes.
        /// Ignora los que ya estén asignados.
        /// </summary>
        public async Task AddPermissionsAsync(
            int roleId,
            IEnumerable<RolePermissionDetailDto> permissions,
            CancellationToken cancellationToken)
        {
            var permissionIds = permissions?
                .Select(p => p.PermissionId)
                .Distinct()
                .ToArray() ?? Array.Empty<int>();

            if (permissionIds.Length == 0)
                return;

            // Obtener los permissions que ya existen para ese rol
            var alreadyAssigned = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);

            var alreadyAssignedSet = alreadyAssigned.ToHashSet();

            // Filtrar solo los que no están aún
            var toAdd = permissionIds
                .Where(id => !alreadyAssignedSet.Contains(id))
                .Select(id => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = id
                })
                .ToList();

            if (toAdd.Count == 0)
                return;

            await _dbContext.RolePermissions.AddRangeAsync(toAdd, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Devuelve los permisos asignados a un rol, proyectados a DTO.
        /// </summary>
        public async Task<IReadOnlyList<RolePermissionDto>> GetPermissionsByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => new RolePermissionDto
                {
                    PermissionId = rp.PermissionId,
                    PermissionName = rp.Permission.Name
                })
                .ToListAsync(cancellationToken);
        }


        /// <summary>
        /// Elimina del rol los permisos indicados.
        /// Si algún permiso no está asignado, se ignora.
        /// </summary>
        public async Task RemovePermissionsAsync(
            int roleId,
            IEnumerable<RolePermissionDetailDto> permissions,
            CancellationToken cancellationToken)
        {
            var permissionIds = permissions?
                .Select(p => p.PermissionId)
                .Distinct()
                .ToArray() ?? Array.Empty<int>();

            if (permissionIds.Length == 0)
                return;

            var toRemove = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .ToListAsync(cancellationToken);

            _dbContext.RolePermissions.RemoveRange(toRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Reemplaza completamente la lista de permisos de un rol.
        /// </summary>
        public async Task UpdateAsync(
            int roleId,
            int[] newPermissionIds,
            CancellationToken cancellationToken)
        {
            var newPermissionSet = new HashSet<int>(newPermissionIds ?? Array.Empty<int>());

            var currentPermissions = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(cancellationToken);

            // 1. Eliminar los permisos que ya no están en la nueva lista
            _dbContext.RolePermissions.RemoveRange(
                currentPermissions.Where(rp => !newPermissionSet.Contains(rp.PermissionId)));

            // 2. Insertar solo los que faltan
            var currentPermissionSet = currentPermissions.Select(rp => rp.PermissionId).ToHashSet();
            var toAdd = newPermissionSet
                .Where(permissionId => !currentPermissionSet.Contains(permissionId))
                .Select(permissionId => new RolePermission
                {
                    PermissionId = permissionId,
                    RoleId = roleId
                });

            await _dbContext.RolePermissions.AddRangeAsync(toAdd, cancellationToken);

            // 3. Un solo SaveChanges: el cambio es atómico
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
