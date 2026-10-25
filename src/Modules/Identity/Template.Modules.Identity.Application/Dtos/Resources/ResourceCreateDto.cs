using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Resources;

public class ResourceCreateDto : ICreateMapTo<Resource>
{
    public string Name { get; set; }

    public Resource ToEntity()
    {
        return new Resource
        {
            Name = Name
        };
    }
}
