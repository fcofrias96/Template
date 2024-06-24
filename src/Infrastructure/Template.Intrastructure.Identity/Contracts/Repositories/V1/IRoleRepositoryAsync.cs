using System.Security.Claims;
using Template.Application.Models.DTOs.V1.Role;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IRoleRepositoryAsync
    {
        Task<List<RoleListDTO>> GetAllAsync();

        Task<List<Claim>> GetClaims(string roleName);
    }
}