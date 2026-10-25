using System.Security.Claims;

namespace Template.Modules.Identity.Application.Models
{
    public sealed class UserAuthCacheEntry
    {
        public IReadOnlyList<Claim> Claims { get; init; } = Array.Empty<Claim>();
        public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    }
}
