using Template.BuildingBlocks.Application.Utilities;

namespace Template.BuildingBlocks.Application.Contracts
{
    public interface IQueryParametersBase
    {
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }
        int Offset { get; }
        int Limit { get; }

        public List<Field> BuildFields();
    }
}
