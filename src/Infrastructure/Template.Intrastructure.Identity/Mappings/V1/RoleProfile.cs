using AutoMapper;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {            CreateMap<Role, RoleListDto>();
            CreateMap<Role, RoleDetailDto>()
            .ForMember(dest => dest.Resources, opt =>
                opt.MapFrom(src => src.RolePermissions
                    .GroupBy(rp => rp.Permission.Resource)
                    .Select(g => new ResourcePermissionDto
                    {
                        Id = g.Key.Id,
                        Resource = g.Key.Name,
                        Permissions = g.Select(rp => new PermissionListDto
                        {
                            Id = rp.Permission.Id,
                            Name = rp.Permission.Action.Name
                        }).ToArray()
                    })
                    .ToArray()));
        }
    }
}