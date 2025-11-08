using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Extensions;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class CreateServiceAsync<TEntity, TDbContext> : ICreateServiceAsync<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ICreateRepositoryAsync<TEntity, TDbContext> _createRepositoryAsync;

        public CreateServiceAsync(ICreateRepositoryAsync<TEntity, TDbContext> createRepositoryAsync, IMapper mapper, IMemoryCache cache)
        {
            _createRepositoryAsync = createRepositoryAsync;
            _mapper = mapper;
            _cache = cache;
        }

        public virtual async Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TEntity>(source);
            await _createRepositoryAsync.CreateAsync(entity, cancellationToken);
            _cache.InvalidateCache<TEntity>();
            return entity;
        }
    }
}