using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Template.Core.Application.Contracts;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.Utils;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Exceptions;

namespace Template.Core.Application.Services
{
    public class GenericServiceAsync<TEntity> : IGenericServiceAsync<TEntity>
    {
        #region Private Variable

        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly IGenericRepositoryAsync<TEntity> _repository;

        #endregion Private Variable

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

        public virtual async Task<List<TDestination>> GetAllAsync<TDestination>()
        {
            return await _repository.GetAllAsync<TDestination>();
        }

        public virtual async Task<PagedResult<TDestination>> GetAllWithPaginationAsync<TDestination, TSpecification>(IRequest request)
        where TSpecification : ISpecification<TEntity>
        {


            var type = typeof(TSpecification);
            object[] constructorArguments = new object[] { request };
            var spec = (TSpecification)Activator.CreateInstance(type, constructorArguments)!;


            if (spec.Skip % spec.Take != 0)
            {
                throw new BadRequestException($"The 'offset' value ({spec.Skip}) must be either zero or a multiple of the 'limit' value({spec.Take}).");
            }

            var total =  await _repository.GetTotalCountAsync(spec.Criteria);

            if (total < spec.Skip)
            {
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({spec.Take}).");
            }

            var items = await _repository.GetAllAsync<TDestination>(spec);
            var href = _httpContext?.HttpContext?.Request.GetEncodedUrl()!;
            var next = UrlUtile.GetNextURL(href, spec.Take, spec.Skip, total);
            var prev = UrlUtile.GetPrevURL(href, spec.Take, spec.Skip, total);

            return new PagedResult<TDestination>
            {
                Href = href,
                Items = items,
                Limit = spec.Take,
                Next = next,
                Offset = spec.Skip,
                Prev = prev,
                Total = total
            };
        }

        public async Task<(List<TDestination>, string)> GetAllWithETagAsync<TDestination>()
        {
            string cacheKey = $"List_{typeof(TEntity).FullName}";

            if (!_cache.TryGetValue(cacheKey, out List<TDestination> mappedEntities))
            {
                mappedEntities = await _repository.GetAllAsync<TDestination>();

                _cache.Set(cacheKey, mappedEntities, TimeSpan.FromHours(24));
            }

            var eTag = CacheUtils.GenerateETag(mappedEntities);
            return (mappedEntities, eTag);
        }
        public virtual async Task<TDestination> GetByIdAsync<TKey, TDestination>(TKey id)
        {
            var mappedEntity = await _repository.GetByIdAsync<TKey, TDestination> (id);
            if (mappedEntity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id != null ? id : "No Key Provided");
            }

            return mappedEntity;
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