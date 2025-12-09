namespace Template.Application.Models.DTOs
{
    public sealed class EtagListResult<T>
    {
        public IReadOnlyList<T>? Items { get; init; }
        public string Etag { get; init; } = string.Empty;
        public bool IsNotModified { get; init; }
    }

    

}