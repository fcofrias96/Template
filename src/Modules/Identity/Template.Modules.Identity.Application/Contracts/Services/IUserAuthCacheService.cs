using Template.Modules.Identity.Domain.Entities;
using Template.Modules.Identity.Application.Models;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IUserAuthCacheService
    {
        Task<UserAuthCacheEntry> GetUserAuthDataAsync(ApplicationUser user, CancellationToken cancellationToken);
    }
}
