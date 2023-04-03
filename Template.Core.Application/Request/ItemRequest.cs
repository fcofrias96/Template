namespace Template.Core.Application.Request
{
    public class ItemRequest: BaseRequest
    {
        public string? Name { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? Category { get; set; }
    }
}
