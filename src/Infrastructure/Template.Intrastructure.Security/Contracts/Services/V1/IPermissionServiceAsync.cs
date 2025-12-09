using Template.Application.Contracts.Services.Base;
using Template.Security.Dtos.Permissions;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IPermissionServiceAsync : IBaseServiceAsync<Permission>
    {
        Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId);
    }


}