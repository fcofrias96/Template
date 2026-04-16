using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Dtos.RolePermission;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
    {
        private readonly SecurityDbContext _dbContext;

        public RolePermissionRepository(SecurityDbContext dbContext, IConfigurationProvider configurationProvider) : base(dbContext, configurationProvider)
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

            await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .ExecuteDeleteAsync(cancellationToken);
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

            // 1. Eliminar los permisos que ya no están en la nueva lista
            await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId && !newPermissionSet.Contains(rp.PermissionId))
                .ExecuteDeleteAsync(cancellationToken);

            // 2. Obtener los que quedan después del delete (para no reinsertarlos)
            var currentPermissionIds = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);

            var currentPermissionSet = currentPermissionIds.ToHashSet();

            // 3. Insertar solo los que faltan
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
