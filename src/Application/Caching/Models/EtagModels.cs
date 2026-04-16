namespace Template.Application.Caching.Models
{
    // =========================
    // ETag models
    // =========================

    public readonly record struct EtagMetadata(
        string Etag);

    public readonly record struct EtagResult<T>(
        bool IsNotModified,
        string Etag,
        T? Data);
}
