using Template.Domain.Entities.V1;
using Template.Security.Entities;
using Template.Security.Services.V1;

namespace Template.Security.Contracts.Services
{
    public interface IUserAuthCacheService
    {
        Task<UserAuthCacheEntry> GetUserAuthDataAsync(ApplicationUser user, CancellationToken cancellationToken);
    }
}