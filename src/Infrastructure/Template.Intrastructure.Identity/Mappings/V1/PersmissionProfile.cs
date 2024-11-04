using AutoMapper;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {            CreateMap<Permission, PermissionListDto>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Action.Name} {src.Resource.Name}" ));
            CreateMap<PermissionCreateDto, Permission>();
            CreateMap<PermissionUpdateDto, Permission>();
            CreateMap<Permission, PermissionDetailDto>()
               .ForMember(dest => dest.Resource, opt => opt.MapFrom(src => src.Resource.Name))
               .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.Action.Name))
               .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.RolePermissions
                                                                .Select(rp => rp.Role.Name)));
        }
    }
}