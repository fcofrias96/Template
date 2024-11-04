using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class UpdateServiceAsync<TEntity, TDbContext> : IUpdateServiceAsync<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IUpdateRepositoryAsync<TEntity, TDbContext> _updateRepositoryAsync;
        private readonly IReadServiceAsync<TEntity, TDbContext> _readService;

        public UpdateServiceAsync(
            IMemoryCache cache,
            IMapper mapper,
            IAsyncRepository<TEntity, TDbContext> repository,
            IReadServiceAsync<TEntity, TDbContext> readService

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