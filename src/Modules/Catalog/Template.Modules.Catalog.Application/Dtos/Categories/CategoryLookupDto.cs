using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories
{
    public sealed class CategoryLookupDto : LookupDto, IProjection<Category, CategoryLookupDto>
    {
        public static Expression<Func<Category, CategoryLookupDto>> Projection => x => new CategoryLookupDto
        {
            Id = x.Id.ToString(),
            Name = x.Name
        };
    }
}
