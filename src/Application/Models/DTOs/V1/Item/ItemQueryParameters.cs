using System.ComponentModel.DataAnnotations;
using Template.Application.Attributes;
using Template.Application.Utilities;
using item = Template.Domain.Entities.V1.Item;

namespace Template.Application.Models.DTOs.V1.Item
{
    public class ItemQueryParameters : QueryParametersBase<item>
    {
        [AllowedValues("Name", "Price", "Description", null, ErrorMessage = "Values allowed are: Name, Price, Description")]
        public override string? SortBy { get; set; }
        public override string[] FilterableProperties { get; set;} = { "CategoryId", "Description", "Price", "Created" };

        [NumberRangeFormat]
        public string Price { get; set; }

        [DateRangeFormat]
        public string Created { get; set; }

    }
}