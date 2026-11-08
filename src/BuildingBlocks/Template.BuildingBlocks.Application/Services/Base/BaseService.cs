using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Template.BuildingBlocks.Application.Caching;
using Template.BuildingBlocks.Application.Caching.Models;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Contracts.Repositories.Base;
using Template.BuildingBlocks.Application.Contracts.Services.Base;
using Template.BuildingBlocks.Application.Errors;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Application.Utilities;
using Template.SharedKernel.Domain;
using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Application.Services.Base
{
    public abstract class BaseService<TEntity> : ExportService<TEntity>, IBaseService<TEntity>
        where TEntity : class, IEntityBase
    {
        private readonly ICacheService _cache;
        private readonly IBaseRepository<TEntity> _repositoryAsync;
        private readonly IHttpContextAccessor _httpContext;

        protected virtual TimeSpan ListDataTtl => TimeSpan.FromMinutes(5);
        protected virtual TimeSpan PageDataTtl => TimeSpan.FromMinutes(5);
        protected virtual string EntityName => typeof(TEntity).Name;

        #region Cache keys and regions

        // Every list/page of the entity lives in the entity region; each item in its own region.
        // Keys include the DTO type so different projections of the same data never collide.

        protected string EntityRegion => EntityName;

        protected string ItemRegion(int id) => $"{EntityName}:{id}";

        private string ListAllKey<TDestination>() => $"{EntityName}:list:all:{typeof(TDestination).Name}";

        private string ListByScopeKey<TDestination>(string scopeName, int scopeId)
            => $"{EntityName}:list:{scopeName}:{scopeId}:{typeof(TDestination).Name}";

        private string PageKey<TDestination>(string scopeName, int? scopeId, int skip, int take, string? sort, string? filterSignature)
            => $"{EntityName}:page:{scopeName}:{scopeId?.ToString() ?? "none"}:{skip}:{take}:{sort ?? "none"}:{filterSignature ?? "none"}:{typeof(TDestination).Name}";

        private string ItemKey<TDestination>(int id) => $"{EntityName}:item:{id}:{typeof(TDestination).Name}";

        protected virtual void InvalidateAllLists() => _cache.RemoveRegion(EntityRegion);

        protected virtual void InvalidateItem(int id) => _cache.RemoveRegion(ItemRegion(id));

        #endregion Cache keys and regions

        protected BaseService(
            IBaseRepository<TEntity> repository,
            ICacheService cache,
            IHttpContextAccessor httpContext)
            : base(repository)
        {
            _repositoryAsync = repository ?? throw new ArgumentNullException(nameof(repository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
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

        public async Task<Result> SoftDeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entityResult = await GetByIdAsync(id, cancellationToken);
            if (entityResult.IsFailure)
            {
                return entityResult.Error;
            }

            var entity = entityResult.Value;

            await _repositoryAsync.SoftDeleteAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);

            return Result.Success();
        }

        public virtual async Task<Result> PermanentDeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entityResult = await GetByIdAsync(id, cancellationToken);
            if (entityResult.IsFailure)
            {
                return entityResult.Error;
            }

            var entity = entityResult.Value;

            await _repositoryAsync.HardDeletesAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);

            return Result.Success();
        }

        public async Task<Result<TEntity>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _repositoryAsync.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                return EntityErrors.NotFound(EntityName, id);
            }

            return Result.Success(entity);
        }

        protected virtual async Task<Result<TEntity>> GetValidatedEntityForUpdateAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
            where TSource : class, IHasId
        {
            ArgumentNullException.ThrowIfNull(source);

            if (id <= 0)
            {
                return EntityErrors.InvalidId;
            }

            if (source.Id != id)
            {
                return EntityErrors.IdMismatch;
            }

            return await GetByIdAsync(id, cancellationToken);
        }

        public virtual async Task<Result> UpdateAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
            where TSource : class, IHasId, IUpdateMapTo<TEntity>
        {
            var entityResult = await GetValidatedEntityForUpdateAsync(id, source, cancellationToken);
            if (entityResult.IsFailure)
            {
                return entityResult.Error;
            }

            var entity = entityResult.Value;

            source.ApplyTo(entity);

            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);

            return Result.Success();
        }

        public virtual async Task<Result> PatchAsync<TSource>(
            int id,
            TSource source,
            CancellationToken cancellationToken)
            where TSource : class, IHasId, IUpdateMapTo<TEntity>
        {
            var entityResult = await GetValidatedEntityForUpdateAsync(id, source, cancellationToken);
            if (entityResult.IsFailure)
            {
                return entityResult.Error;
            }

            var entity = entityResult.Value;

            source.ApplyTo(entity);

            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
            await InvalidateAfterWriteAsync(entity, cancellationToken);

            return Result.Success();
        }

        public virtual async Task<Result<TDestination>> GetByIdProjectedAsync<TDestination>(
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
                return EntityErrors.NotFound(EntityName, id);
            }

            return Result.Success(mappedEntity);
        }

        public async Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            var items = await _cache.GetOrCreateAsync(
                ListAllKey<TDestination>(),
                ListDataTtl,
                ct => _repositoryAsync.GetProjectedAsync(TDestination.Projection, ct),
                EntityRegion,
                cancellationToken);

            return new ApiResponse<IReadOnlyList<TDestination>>(items);
        }

        public async Task<EtagResult<IReadOnlyList<TDestination>>> GetProjectedWithEtagAsync<TDestination>(
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            return await GetWithEtagAsync(
                ListAllKey<TDestination>(),
                ListDataTtl,
                EntityRegion,
                getEtag: async ct =>
                {
                    var (lastModifiedAt, count) = await _repositoryAsync.GetListMetadataAsync(cancellationToken: ct);
                    return BuildListEtag(lastModifiedAt, count);
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

            return await GetWithEtagAsync(
                ListByScopeKey<TDestination>(scopeName, scopeId),
                ListDataTtl,
                EntityRegion,
                getEtag: async ct =>
                {
                    var (lastModifiedAt, count) = await _repositoryAsync.GetListMetadataAsync(specification, ct);
                    return BuildListEtag(lastModifiedAt, count);
                },
                getData: ct => _repositoryAsync.GetProjectedAsync(specification, TDestination.Projection, ct),
                cancellationToken);
        }

        public virtual async Task<Result<EtagResult<TDestination>>> GetByIdProjectedWithEtagAsync<TDestination>(
            int id,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var etagResult = await GetWithEtagAsync<TDestination>(
                ItemKey<TDestination>(id),
                ListDataTtl,
                ItemRegion(id),
                loadEntry: async ct =>
                {
                    var dto = await _repositoryAsync.GetByIdProjectedAsync(
                        id,
                        TDestination.Projection,
                        ct);

                    if (dto is null)
                    {
                        return null;
                    }

                    var rowVersion = await _repositoryAsync.GetRowVersionAsync(id, ct);

                    if (rowVersion is null || rowVersion.Length == 0)
                    {
                        return null;
                    }

                    return new EtagEntry<TDestination>(dto, BuildRowVersionEtag(EntityName, id, rowVersion));
                },
                cancellationToken);

            if (etagResult is not { } found)
            {
                return EntityErrors.NotFound(EntityName, id);
            }

            return Result.Success(found);
        }

        public virtual async Task<Result<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination>(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            ArgumentNullException.ThrowIfNull(specification);

            var totalResult = await GetValidatedTotalAsync(specification, cancellationToken);
            if (totalResult.IsFailure)
            {
                return totalResult.Error;
            }

            var items = await _repositoryAsync.GetProjectedPaginatedAsync(
                specification,
                TDestination.Projection,
                cancellationToken);

            return Result.Success(BuildPagedCollection(specification, items, totalResult.Value));
        }

        public virtual async Task<Result<EtagResult<PagedCollection<TDestination>>>> GetProjectedPaginatedWithEtagAsync<TDestination>(
            ISpecification<TEntity> specification,
            string scopeName,
            int? scopeId = default,
            string? sort = default,
            string? filterSignature = default,
            CancellationToken cancellationToken = default)
            where TDestination : IProjection<TEntity, TDestination>
        {
            ArgumentNullException.ThrowIfNull(specification);

            var totalResult = await GetValidatedTotalAsync(specification, cancellationToken);
            if (totalResult.IsFailure)
            {
                return totalResult.Error;
            }

            var total = totalResult.Value;

            var etagResult = await GetWithEtagAsync(
                PageKey<TDestination>(scopeName, scopeId, specification.Skip, specification.Take, sort, filterSignature),
                PageDataTtl,
                EntityRegion,
                getEtag: async ct =>
                {
                    var (lastModifiedAt, count) = await _repositoryAsync.GetListMetadataAsync(specification, ct);
                    return BuildListEtag(lastModifiedAt, count);
                },
                getData: async ct =>
                {
                    var items = await _repositoryAsync.GetProjectedPaginatedAsync(
                        specification,
                        TDestination.Projection,
                        ct);

                    return BuildPagedCollection(specification, items, total);
                },
                cancellationToken);

            return Result.Success(etagResult);
        }

        protected virtual Task InvalidateAfterWriteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            InvalidateAllLists();
            InvalidateItem(entity.Id);
            return Task.CompletedTask;
        }

        #region Pagination

        /// <summary>
        /// Validates the paging window of the specification and returns the total number of matching rows.
        /// </summary>
        private async Task<Result<int>> GetValidatedTotalAsync(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken)
        {
            if (specification.Take <= 0)
            {
                return PaginationErrors.InvalidLimit;
            }

            if (specification.Skip % specification.Take != 0)
            {
                return PaginationErrors.OffsetNotMultipleOfLimit(specification.Skip, specification.Take);
            }

            var total = await _repositoryAsync.GetTotalCountAsync(specification, cancellationToken);

            if (total < specification.Skip)
            {
                return PaginationErrors.OffsetOutOfRange(total, specification.Take);
            }

            return Result.Success(total);
        }

        private PagedCollection<TDestination> BuildPagedCollection<TDestination>(
            ISpecification<TEntity> specification,
            IReadOnlyList<TDestination> items,
            int total)
        {
            // Programming/hosting error, not a client one: these services only run inside an HTTP request.
            var href = _httpContext.HttpContext?.Request.GetEncodedUrl()
                ?? throw new InvalidOperationException("Current HTTP request is not available.");

            return new PagedCollection<TDestination>
            {
                HRef = href,
                Elements = items,
                Limit = specification.Take,
                Next = UrlUtile.GetNextURL(href, specification.Take, specification.Skip, total),
                Offset = specification.Skip,
                Prev = UrlUtile.GetPrevURL(href, specification.Take, specification.Skip),
                Total = total
            };
        }

        #endregion Pagination

        #region ETag

        protected virtual string BuildListEtag(DateTimeOffset lastModifiedAt, int count)
            => $"\"{lastModifiedAt.Ticks}-{count}\"";

        private static string BuildRowVersionEtag(string entityName, int id, byte[] rowVersion)
        {
            var hex = Convert.ToHexString(rowVersion);
            return $"\"{entityName}:{id}:{hex}\"";
        }

        /// <summary>
        /// ETag-aware read of data that always exists (lists and pages).
        /// </summary>
        private async Task<EtagResult<T>> GetWithEtagAsync<T>(
            string key,
            TimeSpan ttl,
            string region,
            Func<CancellationToken, Task<string>> getEtag,
            Func<CancellationToken, Task<T>> getData,
            CancellationToken cancellationToken)
        {
            var result = await GetWithEtagAsync<T>(
                key,
                ttl,
                region,
                loadEntry: async ct => new EtagEntry<T>(await getData(ct), await getEtag(ct)),
                cancellationToken);

            return result!.Value;
        }

        /// <summary>
        /// Caches the data together with its ETag, writes the ETag response header and reports "not modified"
        /// when it matches the request's If-None-Match header. Returns null when <paramref name="loadEntry"/>
        /// finds nothing; a null entry is never served from the cache, so a missing row is looked up again next time.
        /// </summary>
        private async Task<EtagResult<T>?> GetWithEtagAsync<T>(
            string key,
            TimeSpan ttl,
            string region,
            Func<CancellationToken, Task<EtagEntry<T>?>> loadEntry,
            CancellationToken cancellationToken)
        {
            var entry = await _cache.GetOrCreateAsync(
                key,
                ttl,
                loadEntry,
                region,
                cancellationToken);

            if (entry is null)
            {
                return null;
            }

            var httpContext = _httpContext.HttpContext;
            var clientEtag = httpContext?.Request.Headers.IfNoneMatch.FirstOrDefault();

            if (httpContext is not null)
            {
                httpContext.Response.Headers.ETag = entry.Etag;
            }

            if (!string.IsNullOrWhiteSpace(clientEtag) &&
                string.Equals(clientEtag, entry.Etag, StringComparison.Ordinal))
            {
                if (httpContext is not null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status304NotModified;
                }

                return new EtagResult<T>(true, entry.Etag, default);
            }

            return new EtagResult<T>(false, entry.Etag, entry.Data);
        }

        private sealed record EtagEntry<T>(T Data, string Etag);

        #endregion ETag
    }
}
