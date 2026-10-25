using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Permissions
{
    public class PermissionDetailDto : IProjection<Permission, PermissionDetailDto>
    {
        public static Expression<Func<Permission, PermissionDetailDto>> Projection => permission => new PermissionDetailDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            ResourceId = permission.ResourceId,
            Resource = permission.Resource.Name,
            ActionId = permission.ActionId,
            Action = permission.Action.Name,
            Roles = permission.RolePermissions.Select(rp => rp.Role.Name!).ToArray()
        };

        public int Id { get; set; }
        public string Resource { get; set; }
        public int ResourceId { get; set; }
        public string Action { get; set; }
        public int ActionId { get; set; }
        public string[] Roles { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
