using Template.Application.Utilities;

namespace Template.Application.Models.DTOs.V1.Item
{
    public class ItemQueryParameters: QueryParametersBase
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Category { get; set; }
    }
}