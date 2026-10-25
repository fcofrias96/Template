using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Template.BuildingBlocks.Application.Caching
{
    /// <summary>
    /// <see cref="ICacheService"/> over <see cref="IMemoryCache"/>. Regions are implemented with a cancellation token
    /// per region attached to each entry: <see cref="RemoveRegion"/> cancels it, which evicts all of its entries.
    /// <see cref="GetOrCreateAsync{T}"/> serializes loads per key with a <see cref="SemaphoreSlim"/>, so concurrent
    /// misses on the same key run the factory once (no cache stampede) while different keys load in parallel.
    /// Register as a singleton. The lock is per process; several instances would each load once.
    /// </summary>
    public sealed class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _regions = new(StringComparer.Ordinal);

        // Per-key semaphores, reference counted and removed when unused so the table only holds in-flight keys.
        private readonly Dictionary<string, KeyLock> _keyLocks = new(StringComparer.Ordinal);

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            return TryGet(key, out T value) ? value : default;
        }

        public void Set<T>(string key, T value, TimeSpan ttl, string? region = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            if (region is not null)
            {
                var regionToken = _regions.GetOrAdd(region, _ => new CancellationTokenSource()).Token;
                options.AddExpirationToken(new CancellationChangeToken(regionToken));
            }

            _memoryCache.Set(key, value, options);
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            TimeSpan ttl,
            Func<CancellationToken, Task<T>> factory,
            string? region = null,
            CancellationToken cancellationToken = default)
        {
            if (TryGet(key, out T hit))
            {
                return hit;
            }

            var keyLock = AcquireKeyLock(key);
            try
            {
                await keyLock.Semaphore.WaitAsync(cancellationToken);
                try
                {
                    // Another caller may have loaded the value while this one was waiting.
                    if (TryGet(key, out hit))
                    {
                        return hit;
                    }

                    var value = await factory(cancellationToken);
                    Set(key, value, ttl, region);
                    return value;
                }
                finally
                {
                    keyLock.Semaphore.Release();
                }
            }
            finally
            {
                ReleaseKeyLock(key, keyLock);
            }
        }

        public void Remove(string key) => _memoryCache.Remove(key);

        public void RemoveRegion(string region)
        {
            // Not disposed on purpose: a concurrent Set may still read its Token; an already-cancelled token
            // simply makes that entry expire immediately.
            if (_regions.TryRemove(region, out var source))
            {
                source.Cancel();
            }
        }

        private bool TryGet<T>(string key, out T value)
        {
            if (_memoryCache.TryGetValue(key, out var cached) && cached is T typed)
            {
                value = typed;
                return true;
            }

            value = default!;
            return false;
        }

        private KeyLock AcquireKeyLock(string key)
        {
            lock (_keyLocks)
            {
                if (!_keyLocks.TryGetValue(key, out var keyLock))
                {
                    keyLock = new KeyLock();
                    _keyLocks[key] = keyLock;
                }

                keyLock.References++;
                return keyLock;
            }
        }

        private void ReleaseKeyLock(string key, KeyLock keyLock)
        {
            lock (_keyLocks)
            {
                if (--keyLock.References == 0)
                {
                    // Nobody else holds a reference (they are counted under this lock), so it is safe to dispose.
                    _keyLocks.Remove(key);
                    keyLock.Semaphore.Dispose();
                }
            }
        }

        /// <summary>Number of keys currently being loaded or waited on. Exposed for tests.</summary>
        internal int PendingKeyLocks
        {
            get
            {
                lock (_keyLocks)
                {
                    return _keyLocks.Count;
                }
            }
        }

        private sealed class KeyLock
        {
            public SemaphoreSlim Semaphore { get; } = new(1, 1);

            // Guarded by the _keyLocks lock.
            public int References { get; set; }
        }
    }
}
