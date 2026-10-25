namespace Template.BuildingBlocks.Application.Caching.Models
{
    /// <summary>
    /// Result of an ETag-aware read: when the client's If-None-Match matches, <see cref="IsNotModified"/> is true
    /// and <see cref="Data"/> is not sent.
    /// </summary>
    public readonly record struct EtagResult<T>(
        bool IsNotModified,
        string Etag,
        T? Data);
}
