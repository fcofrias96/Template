using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Template.Application.Models.DTOs.V1.Role;

namespace Template.Identity.Mappings.V1
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {            CreateMap<IdentityRole, RoleListDTO>();
        }
    }
}