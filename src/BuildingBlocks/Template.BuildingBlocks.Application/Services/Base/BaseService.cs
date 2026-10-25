using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Template.BuildingBlocks.Application.Caching;
using Template.BuildingBlocks.Application.Caching.Models;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.BuildingBlocks.Application.Contracts.Repositories.Base;
using Template.BuildingBlocks.Application.Contracts.Services.Base;
using Template.BuildingBlocks.Application.Exceptions;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Application.Utilities;
using Template.SharedKernel.Domain;

namespace Template.BuildingBlocks.Application.Services.Base
{
    public abstract class BaseService<TEntity> : ExportService<TEntity>, IBaseService<TEntity>
        where TEntity : class, IEntityBase
    {
        private readonly ICacheService _cache;
        private readonly IBaseRepository<TEntity> _repositoryAsync;
        private readonly IHttpContextAccessor _httpContext;

        protected virtual TimeSpan ListDataTtl => TimeSpan.FromDays(1);
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

        public virtual async Task<EtagResult<TDestination>> GetByIdProjectedWithEtagAsync<TDestination>(
            int id,
            CancellationToken cancellationToken)
            where TDestination : IProjection<TEntity, TDestination>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await GetWithEtagAsync(
                ItemKey<TDestination>(id),
                ListDataTtl,
                ItemRegion(id),
                getEtag: async ct =>
                {
                    var rowVersion = await _repositoryAsync.GetRowVersionAsync(id, ct);

                    if (rowVersion is null || rowVersion.Length == 0)
                    {
                        throw new NotFoundException(typeof(TEntity).Name, id);
                    }

                    return BuildRowVersionEtag(EntityName, id, rowVersion);
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

            return await GetWithEtagAsync(
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

        #region ETag

        protected virtual string BuildListEtag(DateTimeOffset lastModifiedAt, int count)
            => $"\"{lastModifiedAt.Ticks}-{count}\"";

        private static string BuildRowVersionEtag(string entityName, int id, byte[] rowVersion)
        {
            var hex = Convert.ToHexString(rowVersion);
            return $"\"{entityName}:{id}:{hex}\"";
        }

        /// <summary>
        /// Caches the data together with its ETag, writes the ETag response header and reports "not modified"
        /// when it matches the request's If-None-Match header.
        /// </summary>
        private async Task<EtagResult<T>> GetWithEtagAsync<T>(
            string key,
            TimeSpan ttl,
            string region,
            Func<CancellationToken, Task<string>> getEtag,
            Func<CancellationToken, Task<T>> getData,
            CancellationToken cancellationToken)
        {
            var entry = await _cache.GetOrCreateAsync(
                key,
                ttl,
                async ct => new EtagEntry<T>(await getData(ct), await getEtag(ct)),
                region,
                cancellationToken);

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
