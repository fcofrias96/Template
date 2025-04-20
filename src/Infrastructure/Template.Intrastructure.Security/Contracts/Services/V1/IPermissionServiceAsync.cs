using Template.Application.Contracts.Services.Base;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IPermissionServiceAsync : IAsyncServiceAsync<Permission, SecurityDbContext>
    {
        Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId);

        //Task<Permission> CreateAsync(PermissionCreateDto permissionCreate);
    }
}