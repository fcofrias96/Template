using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.Wrappers;
using Template.Application.Utilities;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class BaseServiceAsync<TEntity,TDbContext> : ExportServiceAsync<TEntity>, IBaseServiceAsync<TEntity, TDbContext>
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IBaseRepositoryAsync<TEntity, TDbContext> _repositoryAsync;
        private readonly IHttpContextAccessor _httpContext;

        public BaseServiceAsync(IMemoryCache cache, IMapper mapper, IBaseRepositoryAsync<TEntity, TDbContext> repositoryAsync, IHttpContextAccessor httpContext): base(repositoryAsync)
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
            _cache.InvalidateCache<TEntity>();
            return entity;
        }


        public async Task SoftDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            entity.IsDeleted = true;
            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
            _cache.InvalidateCache<TEntity>();
        }

        public virtual async Task PermanentDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            await _repositoryAsync.DeleteAsync(entity, cancellationToken);
            _cache.InvalidateCache<TEntity>();
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

        public virtual async Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters, CancellationToken cancellationToken)
        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationHelper.CreatorInstance<TSpecification, TEntity>(queryParameters);

            if (spec.Skip % spec.Take != 0)
            {
                throw new BadRequestException($"The 'offset' value ({spec.Skip}) must be either zero or a multiple of the 'limit' value({spec.Take}).");
            }

            var total = await _repositoryAsync.GetTotalCountAsync(spec, cancellationToken);

            if (total < spec.Skip)
            {
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({spec.Take}).");
            }

            var items = await _repositoryAsync.GetProjectedPaginatedAsync<TDestination>(spec, cancellationToken);
            var href = _httpContext?.HttpContext?.Request.GetEncodedUrl()!;
            var next = UrlUtile.GetNextURL(href, spec.Take, spec.Skip, total);
            var prev = UrlUtile.GetPrevURL(href, spec.Take, spec.Skip);

            var pagedCollection = new PagedCollection<TDestination>
            {
                HRef = href,
                Elements = items,
                Limit = spec.Take,
                Next = next,
                Offset = spec.Skip,
                Prev = prev,
                Total = total
            };

            return new ApiResponse<PagedCollection<TDestination>>(pagedCollection);
        }

        public async Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
        {
            string cacheKey = $"{typeof(TEntity).FullName}Cache";

            if (!_cache.TryGetValue(cacheKey, out IReadOnlyList<TDestination> mappedEntities))
            {
                mappedEntities = await _repositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);
                _cache.Set(cacheKey, mappedEntities, TimeSpan.FromHours(24));
            }

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
            _cache.InvalidateCache<TEntity>();
            await _repositoryAsync.UpdateAsync(entity, cancellationToken);
        }
    }
}