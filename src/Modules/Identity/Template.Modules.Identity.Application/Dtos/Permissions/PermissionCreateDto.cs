using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Permissions;

public class PermissionCreateDto : ICreateMapTo<Permission>
{
    public int ActionId { get; set; }
    public int ResourceId { get; set; }

    public Permission ToEntity()
    {
        return new Permission
        {
            ActionId = ActionId,
            ResourceId = ResourceId
        };
    }
}
