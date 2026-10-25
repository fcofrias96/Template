using System.Linq.Expressions;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Dtos.Categories;

public class CategoryCreateDto : ICreateMapTo<Category>
{
    public string Name { get; set; }

    public Category ToEntity()
    {
        return new Category
        {
            Name = this.Name
        };
    }
}
