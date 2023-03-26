using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Template.Core.Application.DTOs.Role;

namespace Template.Infrastructure.Identity.Mappings
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {            CreateMap<RoleListDTO, IdentityRole>().ReverseMap();
        }
    }
}