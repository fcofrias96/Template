using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Template.Core.Application.DTOs.Role;
using Template.Infrastructure.Identity.Contracts.Repositories;

namespace Template.Infrastructure.Identity.Repositories
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

        public async Task<IList<RoleListDTO>> GetAllAsync()
        {
            return await _roleManager.Roles
                .ProjectTo<RoleListDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IList<Claim>> GetClaims(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            return await _roleManager.GetClaimsAsync(role);
        }
    }
}