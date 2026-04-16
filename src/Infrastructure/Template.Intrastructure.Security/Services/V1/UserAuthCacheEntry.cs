using System.Security.Claims;

namespace Template.Security.Services.V1
{
    public sealed class UserAuthCacheEntry
    {
        public IReadOnlyList<Claim> Claims { get; init; } = Array.Empty<Claim>();
        public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    }
}