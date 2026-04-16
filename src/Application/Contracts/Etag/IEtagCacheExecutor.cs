using Template.Application.Caching.Models;

namespace Template.Application.Contracts.Etag
{
    public interface IEtagCacheExecutor
    {
        Task<EtagResult<T>> GetOrCreateAsync<T>(
            string cacheKey,
            TimeSpan ttl,
            Func<CancellationToken, Task<EtagMetadata>> getMetadata,
            Func<CancellationToken, Task<T>> getData,
            string? clientEtag,
            CancellationToken ct);
    }
}