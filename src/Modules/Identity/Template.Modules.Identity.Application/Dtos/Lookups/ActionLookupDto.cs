using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Lookups
{
    public sealed class ActionLookupDto : LookupDto, IProjection<PermissionAction, ActionLookupDto>
    {
        public static Expression<Func<PermissionAction, ActionLookupDto>> Projection => x => new ActionLookupDto
        {
            Id = x.Id.ToString(),
            Name = x.Name
        };
    }
}
