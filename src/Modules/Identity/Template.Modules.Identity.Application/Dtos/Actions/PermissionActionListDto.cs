using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Actions
{
    public class PermissionActionListDto: IProjection<PermissionAction, PermissionActionListDto>
    {
        public static Expression<Func<PermissionAction, PermissionActionListDto>> Projection => action => new PermissionActionListDto
        {
            Id = action.Id,
            Name = action.Name
        };

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
