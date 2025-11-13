using AutoMapper;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Security.Entities;

namespace Template.Security.Mappings.V1
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleListDto>();
            CreateMap<Role, Dropdown>();
            ;
            CreateMap<Role, RoleDetailDto>()
            .ForMember(dest => dest.Resources, opt =>
                opt.MapFrom(src => src.RolePermissions
                    .GroupBy(rp => rp.Permission.Resource)
                    .Select(g => new ResourcePermissionDto
                    {
                        Id = g.Key.Id,
                        Name = g.Key.Name,
                        Permissions = g.Select(rp => new PermissionListDto
                        {
                            Id = rp.Permission.Id,
                            Name = rp.Permission.Name
                        }).ToArray()
                    })
                    .ToArray()))

            .ForMember(dest => dest.Permissions,
            opt => opt.MapFrom(src => src.RolePermissions
                .GroupBy(rp => rp.Permission)
                .Select(p => new PermissionListDto
                {
                    Description = p.Key.Description,
                    Id = p.Key.Id,
                    Name = p.Key.Name
                }).ToArray()));
            CreateMap<RoleCreateDto, Role>();
            CreateMap<RoleUpdateDto, Role>();
        }
    }
}