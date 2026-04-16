using Template.Application.Caching;
using Template.Application.Caching.Models;

public interface ICacheKeyFactory
{
    CacheKeyResult Build<TContext>(CacheKeyDefinition<TContext> def, TContext ctx);
}
