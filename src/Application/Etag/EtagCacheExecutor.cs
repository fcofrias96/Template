using Template.Application.Caching.Models;
using Template.Application.Contracts.Etag;
using Template.Application.Contracts.Services.V1;

namespace Template.Application.Etag
{
    public sealed class EtagCacheExecutor : IEtagCacheExecutor
    {
        private readonly ICacheService _cache;
        public EtagCacheExecutor(ICacheService cache) => _cache = cache;

        public async Task<EtagResult<T>> GetOrCreateAsync<T>(
            string cacheKey,
            TimeSpan ttl,
            Func<CancellationToken, Task<EtagMetadata>> getMetadata,
            Func<CancellationToken, Task<T>> getData,
            string? clientEtag,
            CancellationToken ct)
        {
            var entry = await _cache.GetOrCreateAsync(
                cacheKey,
                ttl,
                async innerCt =>
                {
                    var meta = await getMetadata(innerCt);
                    var data = await getData(innerCt);
                    return new CacheEntry<T>(data, meta.Etag);
                },
                ct);

            if (!string.IsNullOrWhiteSpace(clientEtag) &&
                string.Equals(clientEtag, entry.Etag, StringComparison.Ordinal))
            {
                return new EtagResult<T>(true, entry.Etag, default);
            }

            return new EtagResult<T>(false, entry.Etag, entry.Data);
        }

        private readonly record struct CacheEntry<T>(T Data, string Etag);
    }
}