using Microsoft.AspNetCore.Mvc;
using Template.Application.Attributes;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.DTOs.V1.Items
{
    public class ItemQueryParameters : QueryParametersBase<Item>
    {
        [AllowedValues("Id", "Name", "Price", "Description", null)]
        public override string? SortBy { get; set; }

        [NumberRangeFormat]
        public string? Price { get; set; }

        public string? Description { get; set; }

        [NumberListFormat]
        [FromQuery(Name = "CategoryIds")]
        public string? CategoryId { get; set; }
    }
}