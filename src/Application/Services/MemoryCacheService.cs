using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Services.V1;

namespace Template.Application.Services
{
    public sealed class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out var value))
            {
                return (T?)value;
            }

            return default;
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            _memoryCache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }

}