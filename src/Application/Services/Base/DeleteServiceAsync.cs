using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Extensions;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class DeleteServiceAsync<TEntity, TDbContext> : IDeleteServiceAsync<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly IDeleteRepositoryAsync<TEntity, TDbContext> _deleteRepositoryAsync;
        private readonly IUpdateRepositoryAsync<TEntity, TDbContext> _updateRepositoryAsync;
        private readonly IMemoryCache _cache;
        private readonly IReadServiceAsync<TEntity, TDbContext> _readService;

        public DeleteServiceAsync(
            IDeleteRepositoryAsync<TEntity, TDbContext> deleteRepositoryAsync,
            IUpdateRepositoryAsync<TEntity, TDbContext> updateRepositoryAsync,
            IMemoryCache cache,
            IReadServiceAsync<TEntity, TDbContext> readService
            
            )
        {
            _deleteRepositoryAsync = deleteRepositoryAsync;
            _updateRepositoryAsync = updateRepositoryAsync;
            _cache = cache;
            _readService = readService;
        }

        public async Task SoftDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await _readService.GetByIdAsync(id, cancellationToken);
            entity.IsDeleted = true;
            await _updateRepositoryAsync.UpdateAsync(entity, cancellationToken);
            _cache.InvalidateCache<TEntity>();
        }

        public virtual async Task PermanentDeleteAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var entity = await _readService.GetByIdAsync(id, cancellationToken);
            await _deleteRepositoryAsync.DeleteAsync(entity, cancellationToken);
            _cache.InvalidateCache<TEntity>();
        }
    }
}