namespace Template.Core.Application.Request
{
    public class ItemRequest: BaseRequest
    {
        public string? Name { get; set; }
        public decimal? MinPrice  { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Categories { get; set; }
    }
}
