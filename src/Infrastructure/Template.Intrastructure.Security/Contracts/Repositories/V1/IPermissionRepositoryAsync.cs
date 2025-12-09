using Template.Application.Contracts.Repositories.Base;
using Template.Security.DbContexts.V1;
using Template.Security.Dtos.UserPermission;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IPermissionRepositoryAsync : IBaseRepositoryAsync<Permission>
    {
        Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(string userId);
    }

}