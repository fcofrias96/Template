using AutoMapper;
using Template.Application.Models.DTOs.V1.Permission;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {            CreateMap<Permission, PermissionListDTO>();
            CreateMap<PermissionCreateDTO, Permission>();
            CreateMap<PermissionUpdateDTO, Permission>();
            CreateMap<Permission, PermissionDetailDTO>();
        }
    }
}