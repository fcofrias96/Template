using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Permissions
{
    public class PermissionUpdateDto : IHasId, IUpdateMapTo<Permission>
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int ResourceId { get; set; }

        public void ApplyTo(Permission entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.ActionId = ActionId;
            entity.ResourceId = ResourceId;
        }
    }
}
