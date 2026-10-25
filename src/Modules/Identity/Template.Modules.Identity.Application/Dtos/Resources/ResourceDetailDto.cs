using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Resources
{
    public class ResourceDetailDto : IProjection<Resource, ResourceDetailDto>
    {
        public static Expression<Func<Resource, ResourceDetailDto>> Projection => resource => new ResourceDetailDto
        {
            Id = resource.Id,
            Name = resource.Name,
            Permissions = resource.Permissions.Select(p => p.Name).ToArray()
        };

        public int Id { get; set; }
        public string Name { get; set; }

        public string[] Permissions { get; set; }
    }
}
