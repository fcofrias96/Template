using AutoMapper;
using Template.Application.Models.DTOs.V1.Role;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {            CreateMap<Role, RoleListDto>();
            CreateMap<Role, RoleDetailDto>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions
                                                                .Select(rp => $"{rp.Permission.Action.Name} {rp.Permission.Resource.Name}")));
        }
    }
}