#nullable enable
public interface ICacheNamespaceVersion
{
    long GetOrInit(string nsKey, TimeSpan ttl);
    long Bump(string nsKey, TimeSpan ttl);
}
