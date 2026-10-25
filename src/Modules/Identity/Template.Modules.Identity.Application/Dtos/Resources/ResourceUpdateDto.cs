using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Resources
{
    public class ResourceUpdateDto : IHasId, IUpdateMapTo<Resource>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void ApplyTo(Resource entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.Name = Name;
        }
    }
}
