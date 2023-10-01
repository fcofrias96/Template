using Template.Core.Application.Contracts;

namespace Template.Core.Application.Request
{
    public abstract class BaseRequest : IRequest
    {
        public string OrderBy { get; set; }
        public bool Descending { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; } = 10;
    }
}
