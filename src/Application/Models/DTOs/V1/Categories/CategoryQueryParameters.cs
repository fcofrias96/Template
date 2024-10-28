using Microsoft.AspNetCore.Mvc;
using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;


namespace Template.Application.Models.DTOs.V1.Categories
{
    public class CategoryQueryParameters : QueryParametersBase<Category>
    {
        [AllowedValues("Name","Id", null)]
        public override string? SortBy { get; set; }

        public string? Name { get; set; }

    }
}