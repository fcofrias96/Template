using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Contracts.Services.Base;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class UpdateServiceAsync<TEntity> : IUpdateServiceAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IUpdateRepositoryAsync<TEntity> _updateRepositoryAsync;
        private readonly IReadServiceAsync<TEntity> _readService;

        public UpdateServiceAsync(
            IMemoryCache cache,
            IMapper mapper,
            IAsyncRepository<TEntity> repository,
            IReadServiceAsync<TEntity> readService

            )
        {
            _cache = cache;
            _mapper = mapper;
            _updateRepositoryAsync = repository;
            _readService = readService;
        }

        public virtual async Task UpdateAsync<TKey, TSource>(TKey id, TSource source)
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

            var entity = await _readService.GetByIdAsync(id);
            _mapper.Map(source, entity);
            _cache.InvalidateCache<TEntity>();
            await _updateRepositoryAsync.UpdateAsync(entity);
        }
    }
}