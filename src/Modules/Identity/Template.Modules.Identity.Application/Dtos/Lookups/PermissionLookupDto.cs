using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Lookups
{
    public sealed class PermissionLookupDto : LookupDto, IProjection<Permission, PermissionLookupDto>
    {
        public static Expression<Func<Permission, PermissionLookupDto>> Projection => x => new PermissionLookupDto
        {
            Id = x.Id.ToString(),
            Name = x.Name
        };
    }
}
