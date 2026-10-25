using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Lookups
{
    public sealed class RoleLookupDto : LookupDto, IProjection<ApplicationRole, RoleLookupDto>
    {
        public static Expression<Func<ApplicationRole, RoleLookupDto>> Projection => x => new RoleLookupDto
        {
            Id = x.Id.ToString(),
            Name = x.Name ?? string.Empty
        };
    }
}
