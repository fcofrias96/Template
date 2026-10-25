namespace Template.BuildingBlocks.Application.Caching
{
    /// <summary>
    /// The only caching service: plain key/value entries with a TTL, optionally grouped in a <c>region</c>
    /// so that related entries (e.g. every list of an entity) can be evicted together with <see cref="RemoveRegion"/>.
    /// </summary>
    public interface ICacheService
    {
        T? Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan ttl, string? region = null);

        Task<T> GetOrCreateAsync<T>(
            string key,
            TimeSpan ttl,
            Func<CancellationToken, Task<T>> factory,
            string? region = null,
            CancellationToken cancellationToken = default);

        void Remove(string key);

        /// <summary>Evicts every entry stored with the given region.</summary>
        void RemoveRegion(string region);
    }
}
