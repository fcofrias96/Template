using Template.Application.Contracts.Repositories.V1;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IPermissionRepositoryAsync : IAsyncRepository<Permission>
    {
        Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId);
    }
}