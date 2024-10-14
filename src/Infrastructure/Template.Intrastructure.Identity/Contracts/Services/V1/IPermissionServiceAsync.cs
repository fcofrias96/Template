using Template.Application.Contracts.Services.V1;
using Template.Application.Models.DTOs.V1.Permission;
using Template.Identity.Entities;


namespace Template.Identity.Contracts.Services.V1
{
    public interface IPermissionServiceAsync : IAsyncServiceAsync<Permission>
    {
        Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId);

        Task<Permission> CreateAsync(PermissionCreateDto permissionCreate);
    }
}