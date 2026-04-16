
using System.Globalization;
using Template.Application.Contracts.Services.V1;

namespace Template.Application.Caching
{

    /// <summary>
    /// ✅ Namespace Version:
    /// - Evita ConcurrentDictionary infinito de locks (memory leak lógico)
    /// - Usa "striped locks" (64 locks) basado en hash del key
    /// - Mantiene tu lógica GetOrInit/Bump
    ///
    /// Nota: si tu ICacheService es Redis, lo ideal es usar INCR atómico.
    /// </summary>
    public class CacheNamespaceVersion : ICacheNamespaceVersion
    {
        private readonly ICacheService _cache;

        // 64 stripes: suficiente para reducir contención y sin crecer infinito
        private static readonly object[] _stripes = Enumerable.Range(0, 64)
            .Select(_ => new object())
            .ToArray();

        public CacheNamespaceVersion(ICacheService cache) => _cache = cache;

        private static object StripeLock(string key)
        {
            var idx = (key.GetHashCode() & int.MaxValue) % _stripes.Length;
            return _stripes[idx];
        }

        public long GetOrInit(string nsKey, TimeSpan ttl)
        {
            var raw = _cache.Get<string>(nsKey);
            if (long.TryParse(raw, out var v) && v > 0)
                return v;

            lock (StripeLock(nsKey))
            {
                raw = _cache.Get<string>(nsKey);
                if (long.TryParse(raw, out v) && v > 0)
                    return v;

                v = 1;
                _cache.Set(nsKey, v.ToString(CultureInfo.InvariantCulture), ttl);
                return v;
            }
        }

        public long Bump(string nsKey, TimeSpan ttl)
        {
            lock (StripeLock(nsKey))
            {
                var current = GetOrInit(nsKey, ttl);
                var next = checked(current + 1);
                _cache.Set(nsKey, next.ToString(CultureInfo.InvariantCulture), ttl);
                return next;
            }
        }
    }
}