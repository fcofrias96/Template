namespace Template.Application.Contracts.Services.V1
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan ttl);
        void Remove(string key);
    }
}