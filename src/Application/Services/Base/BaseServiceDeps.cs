using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Caching;
using Template.Application.Contracts.Etag;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.V1;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public record BaseServiceDeps<TEntity>(
        ICacheService Cache,
        ICacheNamespaceVersion CacheNs,
        ICacheKeyFactory CacheKeyFactory,
        ICachePolicyRegistry Policies,
        IBaseRepository<TEntity> Repository,
        IHttpContextAccessor Http,
        IEtagCacheExecutor EtagCache,
        IEtagHttpResponder EtagHttp
    ) where TEntity : class, IEntityBase;
}