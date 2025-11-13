using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Template.Application.Contracts;
using Template.Application.Contracts.Services.Base;
using Template.Application.Exceptions;
using Template.Application.Models.Wrappers;
using Template.Application.Utilities;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class ReadServiceAsync<TEntity, TDbContext> : IReadServiceAsync<TEntity, TDbContext>
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly Contracts.Repositories.Base.IReadRepositoryAsync<TEntity, TDbContext> _readRepositoryAsync;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public ReadServiceAsync(Contracts.Repositories.Base.IReadRepositoryAsync<TEntity, TDbContext> repository, IHttpContextAccessor httpContext, ILogger logger, IMemoryCache cache)
        {
            _readRepositoryAsync = repository;
            _httpContext = httpContext;
            _logger = logger;
            _cache = cache;
        }

        public virtual async Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            var mappedEntity = await _readRepositoryAsync.GetByIdProjectedAsync<TKey, TDestination>(id, cancellationToken);
            if (mappedEntity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return new ApiResponse<TDestination>(mappedEntity);
        }

        public virtual async Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters, CancellationToken cancellationToken)
        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationHelper.CreatorInstance<TSpecification, TEntity>(queryParameters, _logger);

            if (spec.Skip % spec.Take != 0)
            {
                throw new BadRequestException($"The 'offset' value ({spec.Skip}) must be either zero or a multiple of the 'limit' value({spec.Take}).");
            }

            var total = await _readRepositoryAsync.GetTotalCountAsync(spec, cancellationToken);

            if (total < spec.Skip)
            {
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({spec.Take}).");
            }

            var items = await _readRepositoryAsync.GetProjectedPaginatedAsync<TDestination>(spec, cancellationToken);
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
                mappedEntities = await _readRepositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);
                _cache.Set(cacheKey, mappedEntities, TimeSpan.FromHours(24));
            }

            return new ApiResponse<IReadOnlyList<TDestination>>(mappedEntities);
        }

        public async Task<TEntity> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await _readRepositoryAsync.GetByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return entity;
        }
    }
}