using Template.Application.Contracts.Repositories.Base;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IPermissionRepositoryAsync : IAsyncRepository<Permission, IdentityContext>
    {
        Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId);
    }
}