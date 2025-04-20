using AutoMapper;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Security.Entities;

namespace Template.Security.Mappings.V1
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {            CreateMap<Permission, PermissionListDto>();
            CreateMap<Permission, Dropdown>();
            CreateMap<PermissionCreateDto, Permission>();
            CreateMap<PermissionUpdateDto, Permission>();
            CreateMap<Permission, PermissionDetailDto>()
               .ForMember(dest => dest.Resource, opt => opt.MapFrom(src => src.Resource.Name))
               .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.ActionType.Name))
               .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.RolePermissions
                                                                .Select(rp => rp.Role.Name)));
        }
    }
}