namespace Template.Application.Caching.Models
{
    // =========================
    // Cache results / metadata
    // =========================

    public readonly record struct CacheKeyResult(
        string Key,
        TimeSpan DataTtl);
}
