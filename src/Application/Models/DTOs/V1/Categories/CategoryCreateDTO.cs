using System.Linq.Expressions;
using Template.Application.Contracts.Mapping;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Categories;

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