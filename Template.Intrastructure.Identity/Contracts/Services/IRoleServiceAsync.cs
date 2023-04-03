using Template.Core.Application.DTOs.Role;

namespace Template.Infrastructure.Identity.Contracts.Services
{
    public interface IRoleServiceAsync
    {
        Task<(IList<RoleListDTO>, string)> GetAllWithETagAsync();
    }
}