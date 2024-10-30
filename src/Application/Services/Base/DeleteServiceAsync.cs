using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Extensions;
using Template.Domain.Contracts;

namespace Template.Application.Services.Base
{
    public class DeleteServiceAsync<TEntity> : IDeleteServiceAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly IDeleteRepositoryAsync<TEntity> _deleteRepositoryAsync;
        private readonly IUpdateRepositoryAsync<TEntity> _updateRepositoryAsync;
        private readonly IMemoryCache _cache;
        private readonly IReadServiceAsync<TEntity> _readService;

        public DeleteServiceAsync(
            IDeleteRepositoryAsync<TEntity> deleteRepositoryAsync,
            IUpdateRepositoryAsync<TEntity> updateRepositoryAsync,
            IMemoryCache cache,
            IReadServiceAsync<TEntity> readService
            
            )
        {
            _deleteRepositoryAsync = deleteRepositoryAsync;
            _updateRepositoryAsync = updateRepositoryAsync;
            _cache = cache;
            _readService = readService;
        }

        public async Task SoftDeleteAsync<TKey>(TKey id)
        {
            var entity = await _readService.GetByIdAsync(id);
            entity.IsDeleted = true;
            await _updateRepositoryAsync.UpdateAsync(entity);
            _cache.InvalidateCache<TEntity>();
        }

        public virtual async Task PermanentDeleteAsync<TKey>(TKey id)
        {
            var entity = await _readService.GetByIdAsync(id);
            await _deleteRepositoryAsync.DeleteAsync(entity);
            _cache.InvalidateCache<TEntity>();
        }
    }
}