using AutoMapper;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Security.Entities;

namespace Template.Security.Mappings.V1
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {            CreateMap<Role, RoleListDto>();
            CreateMap<Role, RoleDetailDto>()
            .ForMember(dest => dest.Resources, opt =>
                opt.MapFrom(src => src.RolePermissions
                    .GroupBy(rp => rp.ResourceAction.Resource)
                    .Select(g => new ResourcePermissionDto
                    {
                        Id = g.Key.Id,
                        Resource = g.Key.Name,
                        Permissions = g.Select(rp => new PermissionListDto
                        {
                            Id = rp.ResourceAction.Id,
                            Name = rp.ResourceAction.Action.Name
                        }).ToArray()
                    })
                    .ToArray()));
        }
    }
}