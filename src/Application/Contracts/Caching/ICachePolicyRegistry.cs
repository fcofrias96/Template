using Template.Application.Caching;

namespace Template.Application.Contracts.Caching
{
    public interface ICachePolicyRegistry
    {
        CacheKeyDefinition<TContext> Get<TContext>(string name);
    }
}