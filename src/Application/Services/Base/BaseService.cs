using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Reflection;
using Template.Application.Caching;
using Template.Application.Caching.Contexts;
using Template.Application.Caching.Models;
using Template.Application.Constants;
using Template.Application.Contracts;
using Template.Application.Contracts.Caching;
using Template.Application.Contracts.Etag;
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

        private readonly IMapper _mapper;
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


        protected BaseService(BaseServiceDeps<TEntity> deps): base(deps.Repository)
        {
            _cache = deps.Cache;
            _cacheNs = deps.CacheNs;
            _cacheKeyFactory = deps.CacheKeyFactory;
            _policies = deps.Policies;
            _mapper = deps.Mapper;
            _repositoryAsync = deps.Repository;
            _httpContext = deps.Http;
            _etagCache = deps.EtagCache;
            _etagHttp = deps.EtagHttp;
        }

        // =========================
        // Cache keys (via policies)
        // =========================

        protected virtual CacheKeyResult BuildListAllKey()
            => BuildKey(CachePolicyNames.EntityListAll,
                new EntityListAllContext(EntityName),
                dataTtl: ListDataTtl,
                namespaceTtl: NamespaceTtl);

        protected virtual CacheKeyResult BuildListByScopeKey(string scopeName, int scopeId)
            => BuildKey(CachePolicyNames.EntityListByScope,
                new EntityListByScopeContext(EntityName, scopeName, scopeId),
                dataTtl: ListDataTtl,
                namespaceTtl: NamespaceTtl);

        protected CacheKeyResult BuildKey<TContext>(
            string policyName,
            TContext ctx,
            TimeSpan? dataTtl = null,
            TimeSpan? namespaceTtl = null)
        {
            var def = _policies.Get<TContext>(policyName);

            if (dataTtl is not null || namespaceTtl is not null)
            {
                def = new CacheKeyDefinition<TContext>
                {
                    Parts = def.Parts,
                    NamespaceKeys = def.NamespaceKeys,
                    DataTtl = dataTtl ?? def.DataTtl,
                    NamespaceTtl = namespaceTtl ?? def.NamespaceTtl
                };
            }

            return _cacheKeyFactory.Build(def, ctx);
        }

        // =========================
        // CRUD
        // =========================

        public virtual async Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TEntity>(source);
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
                throw new NotFoundException(typeof(TEntity).Name, id);

            return entity;
        }

        public virtual async Task UpdateAsync<TSource>(int id, TSource source, CancellationToken cancellationToken)
        {
            var propertyInfo = typeof(TSource).GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
                throw new BadRequestException("The entity does not have an Id property");

            var sourceId = propertyInfo.GetValue(source);

            if (!EqualityComparer<int?>.Default.Equals(id, (int?)sourceId))
                throw new BadRequestException("Invalid Id used in request");

            var entity = await GetByIdAsync(id, cancellationToken);
            _mapper.Map(source, entity);

            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);
        }

        public virtual async Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TDestination>(int id, CancellationToken cancellationToken)
        {
            var mappedEntity = await _repositoryAsync.GetByIdProjectedAsync<TDestination>(id, cancellationToken);
            if (mappedEntity is null)
                throw new NotFoundException(typeof(TEntity).Name, id);

            return new ApiResponse<TDestination>(mappedEntity);
        }

        // =========================
        // Lists (anti-stampede)
        // =========================

        public async Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
        {
            var keyResult = BuildListAllKey();

            var items = await _cache.GetOrCreateAsync(
                keyResult.Key,
                keyResult.DataTtl,
                ct => _repositoryAsync.GetProjectedAsync<TDestination>(ct),
                cancellationToken);

            return new ApiResponse<IReadOnlyList<TDestination>>(items);
        }

        // =========================
        // Centralized ETag flow (cache + headers + 304)
        // =========================

        private async Task<EtagResult<T>> GetWithEtagAsync<T>(
            CacheKeyResult keyResult,
            Func<CancellationToken, Task<EtagMetadata>> getMetadata,
            Func<CancellationToken, Task<T>> getData,
            CancellationToken ct)
        {
            var clientEtag = _etagHttp.GetClientEtag();

            var result = await _etagCache.GetOrCreateAsync(
                cacheKey: keyResult.Key,
                ttl: keyResult.DataTtl,
                getMetadata: getMetadata,
                getData: getData,
                clientEtag: clientEtag,
                ct: ct);

            _etagHttp.Apply(result.Etag, clientEtag);

            return result;
        }

        // =========================
        // Lists with ETag (centralized)
        // =========================

        public async Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(
            CancellationToken cancellationToken)
        {
            var keyResult = BuildListAllKey();

            return await GetWithEtagAsync(
                keyResult,
                getMetadata: async ct =>
                {
                    var (lastModified, count) = await _repositoryAsync.GetListMetadataAsync(cancellationToken: ct);
                    return new EtagMetadata(BuildListEtag(lastModified, count));
                },
                getData: ct => _repositoryAsync.GetProjectedAsync<TDestination>(ct),
                ct: cancellationToken);


        }

        public async Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(
            ISpecification<TEntity> specification,
            string scopeName,
            int scopeId,
            CancellationToken cancellationToken)
        {
            var keyResult = BuildListByScopeKey(scopeName, scopeId);

            return await GetWithEtagAsync(
                keyResult,
                getMetadata: async ct =>
                {
                    var (lastModified, count) = await _repositoryAsync.GetListMetadataAsync(specification, ct);
                    return new EtagMetadata(BuildListEtag(lastModified, count));
                },
                getData: ct => _repositoryAsync.GetProjectedAsync<TDestination>(specification, ct),
                ct: cancellationToken);


        }

        // =========================
        // ✅ ById with ETag (centralized)
        // =========================

        public virtual async Task<EtagResult<TDestination>> GetByIdProjectedWithEtagAsync<TDestination>(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var itemNsV = _cacheNs.GetOrInit(BuildItemNamespaceKey(id), NamespaceTtl);
            var keyResult = new CacheKeyResult(
                Key: $"{EntityName}:byId:{id}:v{itemNsV}",
                DataTtl: ListDataTtl);

            return await GetWithEtagAsync(
                keyResult,
                getMetadata: async ct =>
                {
                    var rowVersion = await _repositoryAsync.GetRowVersionAsync(id, ct);
                    if (rowVersion is null || rowVersion.Length == 0)
                        throw new NotFoundException(typeof(TEntity).Name, id);

                    var etag = BuildRowVersionEtag(EntityName, id, rowVersion);
                    return new EtagMetadata(etag);
                },
                getData: async ct =>
                {
                    var dto = await _repositoryAsync.GetByIdProjectedAsync<TDestination>(id, ct);
                    if (dto is null)
                        throw new NotFoundException(typeof(TEntity).Name, id);

                    return dto;
                },
                ct: cancellationToken);
        }


        private static string BuildRowVersionEtag(string entityName, int id, byte[] rowVersion)
        {
            var hex = Convert.ToHexString(rowVersion);
            return $"\"{entityName}:{id}:{hex}\"";
        }


        private static string BuildStableEtagFromObject<TDestination>(string entityName, int id, TDestination obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false
            });

            var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json));
            var shortHash = Convert.ToHexString(bytes).Substring(0, 16);
            return $"\"{entityName}:{id}:{shortHash}\"";
        }

        // =========================
        // Pagination (no cache)
        // =========================

        public virtual async Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination>(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken)
        {
            if (specification.Skip % specification.Take != 0)
                throw new BadRequestException($"The 'offset' value ({specification.Skip}) must be either zero or a multiple of the 'limit' value({specification.Take}).");

            var total = await _repositoryAsync.GetTotalCountAsync(specification, cancellationToken);

            if (total < specification.Skip)
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({specification.Take}).");

            var items = await _repositoryAsync.GetProjectedPaginatedAsync<TDestination>(specification, cancellationToken);

            var href = _httpContext?.HttpContext?.Request.GetEncodedUrl()!;
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
        {
            // validaciones igual que tu método actual
            if (specification.Skip % specification.Take != 0)
                throw new BadRequestException($"The 'offset' value ({specification.Skip}) must be either zero or a multiple of the 'limit' value({specification.Take}).");

            var total = await _repositoryAsync.GetTotalCountAsync(specification, cancellationToken);

            if (total < specification.Skip)
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({specification.Take}).");

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
                    var (lastModified, count) = await _repositoryAsync.GetListMetadataAsync(specification, ct);

                    var etag = BuildListEtag(lastModified, count);
                    return new EtagMetadata(etag);
                },
                getData: async ct =>
                {
                    var items = await _repositoryAsync.GetProjectedPaginatedAsync<TDestination>(specification, ct);

                    var href = _httpContext?.HttpContext?.Request.GetEncodedUrl()!;
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
                ct: cancellationToken);
        }



        protected virtual Task InvalidateAfterWriteAsync(TEntity entity, CancellationToken ct)
        {
            InvalidateAllLists();
            return Task.CompletedTask;
        }
    }


}
