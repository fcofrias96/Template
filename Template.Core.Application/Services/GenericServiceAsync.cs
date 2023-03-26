using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.Utils;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Exceptions;

namespace Template.Core.Application.Services
{
    public class GenericServiceAsync<TEntity> : IGenericServiceAsync<TEntity> where TEntity : class
    {
        #region Private Varible
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly IGenericRepositoryAsync<TEntity> _repository;
        #endregion Private Varible

        #region Constructor
        public GenericServiceAsync(IGenericRepositoryAsync<TEntity> repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContext = httpContext;
            _cache = cache;
        }
        #endregion Constructor

        #region Public Methods
        public virtual async Task<TDestination> CreateAsync<TSource, TDestination>(TSource source)
        {
            var entity = _mapper.Map<TEntity>(source);
            await _repository.CreateAsync(entity);
            CacheUtils.InvalidateCache<TEntity>(_cache);
            return _mapper.Map<TDestination>(entity);
        }

        public virtual async Task DeleteAsync<TKey>(TKey id)
        {
            var entity = await GetByIdAsync<TKey, TEntity>(id);

            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            await _repository.DeleteAsync(entity);
            CacheUtils.InvalidateCache<TEntity>(_cache);
        }

        public virtual async Task<bool> ExistsAsync<TKey>(TKey id)
        {
            var entity = await GetByIdAsync<TKey, TEntity>(id);
            return entity != null;
        }

        public async Task<List<TDestination>> GetAllAsync<TDestination>(List<string>? navigationPropertyToInclude)
        {
            string cacheKey = $"List_{typeof(TEntity).FullName}";

            if (!_cache.TryGetValue(cacheKey, out List<TDestination> mappedEntities))
            {
                mappedEntities = await _repository.GetAllAsync<TDestination>(navigationPropertyToInclude);
                _cache.Set(cacheKey, mappedEntities, TimeSpan.FromHours(24));
            }

            return mappedEntities;

        }



        public async Task<(List<TDestination>, string)> GetAllWithETagAsync<TDestination>(List<string>? navigationPropertyToInclude = null)
        {
            string cacheKey = $"List_{typeof(TEntity).FullName}";

            if (!_cache.TryGetValue(cacheKey, out List<TDestination> mappedEntities))
            {
                mappedEntities = await _repository.GetAllAsync<TDestination>(navigationPropertyToInclude);

                _cache.Set(cacheKey, mappedEntities, TimeSpan.FromHours(24));
            }

            var eTag = CacheUtils.GenerateETag(mappedEntities);
            return (mappedEntities, eTag);
        }

        public virtual async Task<PagedResult<TDestination>> GetAllWithPaginationAsync<TDestination>(QueryParameters<TDestination> queryParameters, List<string>? navigationPropertyToInclude = default)
        {

            if(queryParameters.Offset % queryParameters.Limit != 0)
            {
                throw new BadRequestException($"The 'offset' value ({queryParameters.Offset}) must be either zero or a multiple of the 'limit' value({queryParameters.Limit}).");
            }

            var total = await _repository.GetTotalCountAsync();

            if(total < queryParameters.Offset)
            {
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({queryParameters.Limit}).");
            }

            var items = await _repository.GetAllWithPaginationAsync(queryParameters, navigationPropertyToInclude);
            var href = _httpContext?.HttpContext?.Request.GetEncodedUrl();
            var next = UrlUtils.GetNextURL(href, queryParameters.Limit, queryParameters.Offset, total);
            var prev = UrlUtils.GetPrevURL(href, queryParameters.Limit, queryParameters.Offset, total);

            return new PagedResult<TDestination>
            {
                Href = href,
                Items = items,
                Limit = queryParameters.Limit,
                Next = next,
                Offset = queryParameters.Offset,
                Prev = prev,
                Total = total

            };
        }
        public virtual async Task<TDestination> GetByIdAsync<TKey, TDestination>(TKey id, List<string>? navigationPropertyToInclude = default)
        {
            string cacheKey = $"{typeof(TEntity).FullName}";

            if (!_cache.TryGetValue(cacheKey, out TDestination mappedEntity))
            {
                var entity = await _repository.GetByIdAsync(id, navigationPropertyToInclude);
                if (entity is null)
                {
                    throw new NotFoundException(typeof(TEntity).Name, id != null ? id : "No Key Provided");
                }
                mappedEntity = _mapper.Map<TDestination>(entity);

                _cache.Set(cacheKey, mappedEntity, TimeSpan.FromHours(24));

            }
            return mappedEntity;
        }

        public async Task<(TDestination, string)> GetByIdWithETagAsync<TKey, TDestination>(TKey id, List<string>? navigationPropertyToInclude = null)
        {
            string cacheKey = $"{typeof(TEntity).FullName}";

            if (!_cache.TryGetValue(cacheKey, out TDestination mappedEntity))
            {
                var entity = await _repository.GetByIdAsync(id, navigationPropertyToInclude);
                if (entity is null)
                {
                    throw new NotFoundException(typeof(TEntity).Name, id != null ? id : "No Key Provided");
                }
                mappedEntity = _mapper.Map<TDestination>(entity);

                _cache.Set(cacheKey, mappedEntity, TimeSpan.FromHours(24));

            }

            var eTag = CacheUtils.GenerateETag(mappedEntity);
            return (mappedEntity, eTag);
        }

        public virtual async Task UpdateAsync<TKey, TSource>(TKey id, TSource source)
        {
            PropertyInfo? propertyInfo = typeof(TSource).GetProperties().FirstOrDefault(p => p.Name.ToLower() == "id");
            if (propertyInfo is null)
            {
                throw new ArgumentException("The entity does not have an Id property");
            }

            TKey? sourceId = (TKey?)propertyInfo.GetValue(source);

            if (!EqualityComparer<TKey>.Default.Equals(id, sourceId))
            {
                throw new BadRequestException("Invalid Id used in request");
            }

            var entity = await GetByIdAsync<TKey, TEntity>(id);

            if (entity == null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            _mapper.Map(source, entity);
            CacheUtils.InvalidateCache<TEntity>(_cache);
            await _repository.UpdateAsync(entity);
        }
        #endregion Public Methods
    }
}
