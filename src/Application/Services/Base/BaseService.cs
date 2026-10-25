using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using ServiceStack.Model;
using Template.Application.Caching;
using Template.Application.Caching.Contexts;
using Template.Application.Caching.Models;
using Template.Application.Constants;
using Template.Application.Contracts;
using Template.Application.Contracts.Caching;
using Template.Application.Contracts.Etag;
using Template.Application.Contracts.Mapping;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Models.Wrappers;
using Template.Application.Utilities;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public abstract class BaseService<TEntity> : ExportService<TEntity>, IBaseService<TEntity>
        where TEntity : class, IEntityBase
    {
        private readonly ICacheService _cache;
        protected readonly ICacheNamespaceVersion _cacheNs;
        private readonly ICacheKeyFactory _cacheKeyFactory;
        private readonly ICachePolicyRegistry _policies;
        private readonly IBaseRepository<TEntity> _repositoryAsync;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IEtagCacheExecutor _etagCache;
        private readonly IEtagHttpResponder _etagHttp;

        protected virtual TimeSpan ListDataTtl => TimeSpan.FromDays(1);
        protected virtual TimeSpan NamespaceTtl => TimeSpan.FromDays(7);
        protected virtual string EntityName => typeof(TEntity).Name;

        protected virtual string BuildGlobalNamespaceKey() => $"{EntityName}:ns:v";

        protected string BuildScopeNamespaceKey(string scopeName, int scopeId)
            => $"{EntityName}:by:{scopeName}:{scopeId}:ns:v";

        protected virtual void InvalidateAllLists()
            => _cacheNs.Bump(BuildGlobalNamespaceKey(), NamespaceTtl);

        protected virtual string BuildListEtag(DateTimeOffset lastModifiedAt, int count)
            => $"\"{lastModifiedAt.Ticks}-{count}\"";

        protected virtual string BuildItemNamespaceKey(int id)
            => $"{EntityName}:item:{id}:ns:v";

        protected virtual void InvalidateItem(int id)
            => _cacheNs.Bump(BuildItemNamespaceKey(id), NamespaceTtl);

        protected BaseService(BaseServiceDeps<TEntity> deps)
            : base(deps.Repository)
        {
            ArgumentNullException.ThrowIfNull(deps);

            _cache = deps.Cache;
            _cacheNs = deps.CacheNs;
            _cacheKeyFactory = deps.CacheKeyFactory;
            _policies = deps.Policies;
            _repositoryAsync = deps.Repository;
            _httpContext = deps.Http;
            _etagCache = deps.EtagCache;
            _etagHttp = deps.EtagHttp;
        }

        protected virtual CacheKeyResult BuildListAllKey()
            => BuildKey(
                CachePolicyNames.EntityListAll,
                new EntityListAllContext(EntityName),
                dataTtl: ListDataTtl,
                namespaceTtl: NamespaceTtl);

        protected virtual CacheKeyResult BuildListByScopeKey(string scopeName, int scopeId)
            => BuildKey(
                CachePolicyNames.EntityListByScope,
                new EntityListByScopeContext(EntityName, scopeName, scopeId),
                dataTtl: ListDataTtl,
                namespaceTtl: NamespaceTtl);

        protected CacheKeyResult BuildKey<TContext>(
            string policyName,
            TContext context,
            TimeSpan? dataTtl = null,
            TimeSpan? namespaceTtl = null)
        {
            var definition = _policies.Get<TContext>(policyName);

            if (dataTtl is not null || namespaceTtl is not null)
            {
                definition = new CacheKeyDefinition<TContext>
                {
                    Parts = definition.Parts,
                    NamespaceKeys = definition.NamespaceKeys,
                    DataTtl = dataTtl ?? definition.DataTtl,
                    NamespaceTtl = namespaceTtl ?? definition.NamespaceTtl
                };
            }

            return _cacheKeyFactory.Build(definition, context);
        }

        public virtual async Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken)
            where TSource : ICreateMapTo<TEntity>
        {
            ArgumentNullException.ThrowIfNull(source);

            var entity = source.ToEntity();

            await _repositoryAsync.CreateAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);

            return entity;
        }

        public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);

            await _repositoryAsync.SoftDeleteAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);
        }

        public virtual async Task PermanentDeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);

            await _repositoryAsync.HardDeletesAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _repositoryAsync.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return entity;
        }

        protected virtual async Task<TEntity> GetValidatedEntityForUpdateAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
            where TSource : class, IHasId
        {
            ArgumentNullException.ThrowIfNull(source);

            if (id <= 0)
            {
                throw new BadRequestException("The request id must be greater than zero.");
            }

            if (source.Id != id)
            {
                throw new BadRequestException("The route id does not match the payload id.");
            }

            return await GetByIdAsync(id, cancellationToken);
        }

        public virtual async Task UpdateAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
            where TSource : class, IHasId, IUpdateMapTo<TEntity>
        {
            var entity = await GetValidatedEntityForUpdateAsync(id, source, cancellationToken);

            source.ApplyTo(entity);

            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);
        }

        public virtual async Task PatchAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
            where TSource : class, IHasId, IUpdateMapTo<TEntity>
        {
            var entity = await GetValidatedEntityForUpdateAsync(id, source, cancellationToken);

            source.ApplyTo(entity);

            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);
        }

        public virtual async Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TDestination>(
            int id,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            var mappedEntity = await _repositoryAsync.GetByIdProjectedAsync(
                id,
                TDestination.Projection,
                cancellationToken);

            if (mappedEntity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return new ApiResponse<TDestination>(mappedEntity);
        }

        public async Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            var keyResult = BuildListAllKey();

            var items = await _cache.GetOrCreateAsync(
                keyResult.Key,
                keyResult.DataTtl,
                ct => _repositoryAsync.GetProjectedAsync(TDestination.Projection, ct),
                cancellationToken);

            return new ApiResponse<IReadOnlyList<TDestination>>(items);
        }

        private async Task<EtagResult<T>> GetWithEtagAsync<T>(
            CacheKeyResult keyResult,
            Func<CancellationToken, Task<EtagMetadata>> getMetadata,
            Func<CancellationToken, Task<T>> getData,
            CancellationToken cancellationToken)
        {
            var clientEtag = _etagHttp.GetClientEtag();

            var result = await _etagCache.GetOrCreateAsync(
                cacheKey: keyResult.Key,
                ttl: keyResult.DataTtl,
                getMetadata: getMetadata,
                getData: getData,
                clientEtag: clientEtag,
                ct: cancellationToken);

            _etagHttp.Apply(result.Etag, clientEtag);

            return result;
        }

        public async Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            var keyResult = BuildListAllKey();

            return await GetWithEtagAsync(
                keyResult,
                getMetadata: async ct =>
                {
                    var (lastModifiedAt, count) = await _repositoryAsync.GetListMetadataAsync(cancellationToken: ct);
                    return new EtagMetadata(BuildListEtag(lastModifiedAt, count));
                },
                getData: ct => _repositoryAsync.GetProjectedAsync(TDestination.Projection, ct),
                cancellationToken);
        }

        public async Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(
            ISpecification<TEntity> specification,
            string scopeName,
            int scopeId,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            ArgumentNullException.ThrowIfNull(specification);

            var keyResult = BuildListByScopeKey(scopeName, scopeId);

            return await GetWithEtagAsync(
                keyResult,
                getMetadata: async ct =>
                {
                    var (lastModifiedAt, count) = await _repositoryAsync.GetListMetadataAsync(specification, ct);
                    return new EtagMetadata(BuildListEtag(lastModifiedAt, count));
                },
                getData: ct => _repositoryAsync.GetProjectedAsync(specification, TDestination.Projection, ct),
                cancellationToken);
        }

        public virtual async Task<EtagResult<TDestination>> GetByIdProjectedWithEtagAsync<TDestination>(
            int id,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var itemNamespaceVersion = _cacheNs.GetOrInit(BuildItemNamespaceKey(id), NamespaceTtl);
            var keyResult = new CacheKeyResult(
                Key: $"{EntityName}:byId:{id}:v{itemNamespaceVersion}",
                DataTtl: ListDataTtl);

            return await GetWithEtagAsync(
                keyResult,
                getMetadata: async ct =>
                {
                    var rowVersion = await _repositoryAsync.GetRowVersionAsync(id, ct);

                    if (rowVersion is null || rowVersion.Length == 0)
                    {
                        throw new NotFoundException(typeof(TEntity).Name, id);
                    }

                    var etag = BuildRowVersionEtag(EntityName, id, rowVersion);
                    return new EtagMetadata(etag);
                },
                getData: async ct =>
                {
                    var dto = await _repositoryAsync.GetByIdProjectedAsync(
                        id,
                        TDestination.Projection,
                        ct);

                    if (dto is null)
                    {
                        throw new NotFoundException(typeof(TEntity).Name, id);
                    }

                    return dto;
                },
                cancellationToken);
        }

        private static string BuildRowVersionEtag(string entityName, int id, byte[] rowVersion)
        {
            var hex = Convert.ToHexString(rowVersion);
            return $"\"{entityName}:{id}:{hex}\"";
        }

        public virtual async Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination>(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            ArgumentNullException.ThrowIfNull(specification);

            if (specification.Take <= 0)
            {
                throw new BadRequestException("The 'limit' value must be greater than zero.");
            }

            if (specification.Skip % specification.Take != 0)
            {
                throw new BadRequestException(
                    $"The 'offset' value ({specification.Skip}) must be either zero or a multiple of the 'limit' value ({specification.Take}).");
            }

            var total = await _repositoryAsync.GetTotalCountAsync(specification, cancellationToken);

            if (total < specification.Skip)
            {
                throw new BadRequestException(
                    $"The 'offset' value must be either zero or less than or equal to 'total' value ({total}) and a multiple of the 'limit' value ({specification.Take}).");
            }

            var items = await _repositoryAsync.GetProjectedPaginatedAsync(
                specification,
                TDestination.Projection,
                cancellationToken);

            var href = _httpContext.HttpContext?.Request.GetEncodedUrl()
                ?? throw new InvalidOperationException("Current HTTP request is not available.");

            var next = UrlUtile.GetNextURL(href, specification.Take, specification.Skip, total);
            var prev = UrlUtile.GetPrevURL(href, specification.Take, specification.Skip);

            var pagedCollection = new PagedCollection<TDestination>
            {
                HRef = href,
                Elements = items,
                Limit = specification.Take,
                Next = next,
                Offset = specification.Skip,
                Prev = prev,
                Total = total
            };

            return new ApiResponse<PagedCollection<TDestination>>(pagedCollection);
        }

        public virtual async Task<EtagResult<PagedCollection<TDestination>>> GetProjectedPaginatedWithEtagAsync<TDestination>(
            ISpecification<TEntity> specification,
            string scopeName,
            int? scopeId = default,
            string? sort = default,
            string? filterSignature = default,
            CancellationToken cancellationToken = default)
            where TDestination : IProjection<TEntity, TDestination>
        {
            ArgumentNullException.ThrowIfNull(specification);

            if (specification.Take <= 0)
            {
                throw new BadRequestException("The 'limit' value must be greater than zero.");
            }

            if (specification.Skip % specification.Take != 0)
            {
                throw new BadRequestException(
                    $"The 'offset' value ({specification.Skip}) must be either zero or a multiple of the 'limit' value ({specification.Take}).");
            }

            var total = await _repositoryAsync.GetTotalCountAsync(specification, cancellationToken);

            if (total < specification.Skip)
            {
                throw new BadRequestException(
                    $"The 'offset' value must be either zero or less than or equal to 'total' value ({total}) and a multiple of the 'limit' value ({specification.Take}).");
            }

            var keyResult = BuildKey(
                CachePolicyNames.EntityPageByScope,
                new EntityPageContext(
                    EntityName,
                    scopeName,
                    scopeId,
                    specification.Skip,
                    specification.Take,
                    sort,
                    filterSignature),
                dataTtl: TimeSpan.FromMinutes(5),
                namespaceTtl: NamespaceTtl);

            return await GetWithEtagAsync(
                keyResult,
                getMetadata: async ct =>
                {
                    var (lastModifiedAt, count) = await _repositoryAsync.GetListMetadataAsync(specification, ct);
                    return new EtagMetadata(BuildListEtag(lastModifiedAt, count));
                },
                getData: async ct =>
                {
                    var items = await _repositoryAsync.GetProjectedPaginatedAsync(
                        specification,
                        TDestination.Projection,
                        ct);

                    var href = _httpContext.HttpContext?.Request.GetEncodedUrl()
                        ?? throw new InvalidOperationException("Current HTTP request is not available.");

                    var next = UrlUtile.GetNextURL(href, specification.Take, specification.Skip, total);
                    var prev = UrlUtile.GetPrevURL(href, specification.Take, specification.Skip);

                    return new PagedCollection<TDestination>
                    {
                        HRef = href,
                        Elements = items,
                        Limit = specification.Take,
                        Next = next,
                        Offset = specification.Skip,
                        Prev = prev,
                        Total = total
                    };
                },
                cancellationToken);
        }

        protected virtual Task InvalidateAfterWriteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            InvalidateAllLists();
            InvalidateItem(entity.Id);
            return Task.CompletedTask;
        }
    }
}