namespace Template.Core.Application.Contracts
{
    public interface IRequest
    {
        string OrderBy { get; }
        bool Descending { get; }
        int Offset { get; }
        int Limit { get; }
    }
}
