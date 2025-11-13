using Microsoft.AspNetCore.Mvc;
using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Items
{
    public class ItemQueryParameters : QueryParametersBase<Item>
    {
        [AllowedValues("Id", "Name", "Price", "Description", "Category", null, "Created")]
        public override string? SortBy { get; set; }

        [NumberRangeFormat]
        public string? Price { get; set; }

        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }

        [DateRangeFormat]
        public string? CreatedAt { get; set; } //delete

    }
}