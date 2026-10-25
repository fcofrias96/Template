using System.Linq.Expressions;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Application.Models.DTOs.V1.Actions
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