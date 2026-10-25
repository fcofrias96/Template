using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Application.Dtos.Permissions;
using Template.Modules.Identity.Application.Dtos.Resources;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Roles
{
    public class RoleDetailDto : IProjection<ApplicationRole, RoleDetailDto>
    {
        public static Expression<Func<ApplicationRole, RoleDetailDto>> Projection => role => new RoleDetailDto
        {
            Id = role.Id.ToString(),
            Name = role.Name!,
            NormalizedName = role.NormalizedName!,
            Resources = role.RolePermissions
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
                .ToArray(),
            Permissions = role.RolePermissions
                .Select(rp => new PermissionListDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description
                })
                .ToArray()
        };

        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public ResourcePermissionDto[] Resources { get; set; }

        public PermissionListDto[] Permissions { get; set; }
    }
}
