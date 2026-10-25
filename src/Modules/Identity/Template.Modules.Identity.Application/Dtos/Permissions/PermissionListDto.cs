using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Permissions
{
    public class PermissionListDto : IProjection<Permission, PermissionListDto>
    {
        public static Expression<Func<Permission, PermissionListDto>> Projection => permission => new PermissionListDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description
        };

        public int Id { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
