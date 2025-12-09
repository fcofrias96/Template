using Template.Security.Entities;

namespace Template.Security.Contracts.Services
{
    public interface IUserAuthCacheService
    {
        Task<UserAuthCacheEntry> GetUserAuthDataAsync(User user, CancellationToken cancellationToken);
    }
}