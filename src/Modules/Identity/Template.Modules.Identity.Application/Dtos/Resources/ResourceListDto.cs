using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Resources
{
    public class ResourceListDto : IProjection<Resource, ResourceListDto>
    {
        public static Expression<Func<Resource, ResourceListDto>> Projection => resource => new ResourceListDto
        {
            Id = resource.Id,
            Name = resource.Name
        };

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
