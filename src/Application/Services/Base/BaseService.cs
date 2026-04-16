using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Reflection;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Models.DTOs;
using Template.Application.Models.Wrappers;
using Template.Application.Utilities;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public abstract class BaseService<TEntity> : ExportService<TEntity>, IBaseService<TEntity>
        where TEntity : class, IEntityBase
    {
        private readonly ICacheService _cache;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<TEntity> _repositoryAsync;
        private readonly IHttpContextAccessor _httpContext;
        private readonly TimeSpan _ttl = TimeSpan.FromDays(1);
        protected virtual string EntityName => typeof(TEntity).Name;
        protected virtual string BuildListKey()=> $"{EntityName}:list:all";
        protected virtual string BuildListEtag(DateTimeOffset lastModifiedAt, int count)=> $"\"{lastModifiedAt.Ticks}-{count}\"";


        protected BaseService(ICacheService cache, IMapper mapper, IBaseRepository<TEntity> repositoryAsync, IHttpContextAccessor httpContext) : base(repositoryAsync)
        {
            _cache = cache;
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _httpContext = httpContext;
        }

        public virtual async Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TEntity>(source);
            await _repositoryAsync.CreateAsync(entity, cancellationToken);
            _cache.Remove(BuildListKey());
            return entity;
        }

        public async Task SoftDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            entity.MarkDeleted();
            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
            _cache.Remove(BuildListKey());
        }

        public virtual async Task PermanentDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            await _repositoryAsync.DeleteAsync(entity, cancellationToken);
            _cache.Remove(BuildListKey());
        }

        public virtual async Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            var mappedEntity = await _repositoryAsync.GetByIdProjectedAsync<TKey, TDestination>(id, cancellationToken);
            if (mappedEntity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return new ApiResponse<TDestination>(mappedEntity);
        }

        public virtual async Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> specification, CancellationToken cancellationToken)
        {
            if (specification.Skip % specification.Take != 0)
            {
                throw new BadRequestException($"The 'offset' value ({specification.Skip}) must be either zero or a multiple of the 'limit' value({specification.Take}).");
            }

            var total = await _repositoryAsync.GetTotalCountAsync(specification, cancellationToken);

            if (total < specification.Skip)
            {
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({specification.Take}).");
            }

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

        public async Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
        {
            var key = BuildListKey();
            var cached = _cache.Get<IReadOnlyList<TDestination>>(key);

            if (cached is not null)
                return new ApiResponse<IReadOnlyList<TDestination>>(cached);

            var mappedEntities = await _repositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);

            _cache.Set(key, mappedEntities, _ttl);

            return new ApiResponse<IReadOnlyList<TDestination>>(mappedEntities);
        }

        public async Task<TEntity> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await _repositoryAsync.GetByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return entity;
        }

        public virtual async Task UpdateAsync<TKey, TSource>(TKey id, TSource source, CancellationToken cancellationToken)
        {
            var propertyInfo = typeof(TSource).GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new BadRequestException("The entity does not have an Id property");
            }

            var sourceId = propertyInfo.GetValue(source);

            if (!EqualityComparer<TKey>.Default.Equals(id, (TKey?)sourceId))
            {
                throw new BadRequestException("Invalid Id used in request");
            }

            var entity = await GetByIdAsync(id, cancellationToken);
            _mapper.Map(source, entity);
            _cache.Remove(BuildListKey());
            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
        }

        public async Task<EtagListResult<TDestination>> GetProjectedWithEtagAsync<TDestination>(
            CancellationToken cancellationToken)
        {
            var key = BuildListKey();
            var clientEtag = _httpContext.HttpContext?.Request.Headers["If-None-Match"].FirstOrDefault();

            // 1. Intentar leer del cache (data + etag)
            var cached = _cache.Get<ListCacheEntry<TDestination>>(key);

            if (cached is not null)
            {
                // Si el cliente envió ETag y coincide → no modificado
                if (!string.IsNullOrWhiteSpace(clientEtag) &&
                    string.Equals(clientEtag, cached.Etag, StringComparison.Ordinal))
                {
                    return new EtagListResult<TDestination>
                    {
                        IsNotModified = true,
                        Etag = cached.Etag,
                        Items = Array.Empty<TDestination>()
                    };
                }

                // Si no coincide → devolvemos data del cache con el ETag actual
                return new EtagListResult<TDestination>
                {
                    IsNotModified = false,
                    Etag = cached.Etag,
                    Items = cached.Data
                };
            }

            // 2. No hay cache → vamos a repo

            // 2.1. Traemos metadata (para ETag)
            var (lastModified, count) = await _repositoryAsync
                .GetListMetadataAsync(cancellationToken);

            var etag = BuildListEtag(lastModified, count);

            // 2.2. Traemos la proyección
            var mappedEntities = await _repositoryAsync
                .GetProjectedAsync<TDestination>(cancellationToken);

            var entry = new ListCacheEntry<TDestination>
            {
                Data = mappedEntities,
                Etag = etag,
                LastModifiedAt = lastModified,
                Count = count
            };

            _cache.Set(key, entry, _ttl);

            return new EtagListResult<TDestination>
            {
                IsNotModified = false,
                Etag = etag,
                Items = mappedEntities
            };
        }

        private sealed class ListCacheEntry<T>
        {
            public IReadOnlyList<T> Data { get; init; } = Array.Empty<T>();
            public string Etag { get; init; } = string.Empty;
            public DateTimeOffset LastModifiedAt { get; init; }
            public int Count { get; init; }
        }
    }
}