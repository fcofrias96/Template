using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Roles
{
    public class RoleListDto : IProjection<ApplicationRole, RoleListDto>
    {
        public static Expression<Func<ApplicationRole, RoleListDto>> Projection => role => new RoleListDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty
        };

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
