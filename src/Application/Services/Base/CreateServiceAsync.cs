using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Extensions;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class CreateServiceAsync<TEntity> : ICreateServiceAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ICreateRepositoryAsync<TEntity> _createRepositoryAsync;

        public CreateServiceAsync(ICreateRepositoryAsync<TEntity> createRepositoryAsync, IMapper mapper, IMemoryCache cache)
        {
            _createRepositoryAsync = createRepositoryAsync;
            _mapper = mapper;
            _cache = cache;
        }

        public virtual async Task<TEntity> CreateAsync<TSource>(TSource source)
        {
            var entity = _mapper.Map<TEntity>(source);
            await _createRepositoryAsync.CreateAsync(entity);
            _cache.InvalidateCache<TEntity>();
            return entity;
        }
    }
}