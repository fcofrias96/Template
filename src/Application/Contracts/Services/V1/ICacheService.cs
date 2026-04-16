namespace Template.Application.Contracts.Services.V1
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan ttl);
        void Remove(string key);

        Task<T> GetOrCreateAsync<T>(
            string key,
            TimeSpan ttl,
            Func<CancellationToken, Task<T>> factory,
            CancellationToken ct = default);
    }
}