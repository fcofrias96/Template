using Microsoft.AspNetCore.Http;
using Template.Security.Contracts.Services;

namespace Template.Security.Services.V1
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public int? UserId =>
            int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value, out var userId) ? userId : (int?)null;
    }
}