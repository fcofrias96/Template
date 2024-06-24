using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Template.Application.Models.DTOs.V1.Role;
using Template.Identity.Contracts.Repositories.V1;

namespace Template.Identity.Repositories.V1
{
    public class RoleRepositoryAsync : IRoleRepositoryAsync
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleRepositoryAsync(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<List<RoleListDTO>> GetAllAsync()
        {
            return await _roleManager.Roles
                .ProjectTo<RoleListDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<Claim>> GetClaims(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            return (List<Claim>)await _roleManager.GetClaimsAsync(role);
        }
    }
}