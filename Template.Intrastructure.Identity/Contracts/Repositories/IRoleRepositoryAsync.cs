using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Template.Core.Application.DTOs.Role;

namespace Template.Infrastructure.Identity.Contracts.Repositories
{
    public interface IRoleRepositoryAsync
    {
        Task<IList<RoleListDTO>> GetAllAsync();

        Task<IList<Claim>> GetClaims(string roleName);


    }
}
