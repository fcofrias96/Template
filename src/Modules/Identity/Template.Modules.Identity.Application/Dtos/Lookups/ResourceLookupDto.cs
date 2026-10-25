using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Lookups
{
    public sealed class ResourceLookupDto : LookupDto, IProjection<Resource, ResourceLookupDto>
    {
        public static Expression<Func<Resource, ResourceLookupDto>> Projection => x => new ResourceLookupDto
        {
            Id = x.Id.ToString(),
            Name = x.Name
        };
    }
}
