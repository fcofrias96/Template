namespace Template.Security.Contracts.Repositories.V1
{
    public interface IUserRoleRepository
    {
        /// <summary>
        /// Obtiene todos los IDs de roles asignados a un usuario.
        /// </summary>
        Task<IReadOnlyList<string>> GetRoleIdsByUserIdAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Actualiza la lista de roles de un usuario:
        /// - Agrega los nuevos
        /// - Quita los que ya no están en roleIds
        /// </summary>
        Task UpdateUserRolesAsync(string userId, IEnumerable<string> roleIds, CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene todos los IDs de usuarios que tienen asignado un rol.
        /// Sirve para invalidar cache cuando cambian los permisos de un rol.
        /// </summary>
        Task<IReadOnlyList<string>> GetUserIdsByRoleIdAsync(string roleId, CancellationToken cancellationToken);
    }


}