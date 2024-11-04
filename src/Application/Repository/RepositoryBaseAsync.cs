using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class RepositoryBaseAsync<TEntity, TDbContext> : IAsyncRepository<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        #region Private Variable

        private readonly ICreateRepositoryAsync<TEntity, TDbContext> _createRepository;
        private readonly IDeleteRepositoryAsync<TEntity, TDbContext> _deleteRepository;
        private readonly IReadRepositoryAsync<TEntity, TDbContext> _readRepository;
        private readonly IUpdateRepositoryAsync<TEntity, TDbContext> _updateRepository;
        public RepositoryBaseAsync(
            ICreateRepositoryAsync<TEntity, TDbContext> createRepository, 
            IUpdateRepositoryAsync<TEntity, TDbContext> updateRepository, 
            IDeleteRepositoryAsync<TEntity, TDbContext> deleteRepository, 
            IReadRepositoryAsync<TEntity, TDbContext> readRepository
            )
        {
            _createRepository = createRepository;
            _updateRepository = updateRepository;
            _deleteRepository = deleteRepository;
            _readRepository = readRepository;
        }

        #endregion Private Variable

        public async Task CreateAsync(TEntity entity)
        {
          await _createRepository.CreateAsync(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
           await _deleteRepository.DeleteAsync(entity);
        }

        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id)
        {
            return await _readRepository.GetByIdAsync(id);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            return await _readRepository.GetByIdProjectedAsync<TKey, TDestination>(id);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>()
        {
           return await _readRepository.GetProjectedAsync<TDestination>();
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec)
        {
            return await _readRepository.GetProjectedForExportAsync<TDestination>(spec);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec)
        {
            return await _readRepository.GetProjectedPaginatedAsync<TDestination>(spec);
        }

        public async Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria)
        {
            return await _readRepository.GetTotalCountAsync(criteria);
        }

        public async Task UpdateAsync(TEntity entity)
        {
           await _updateRepository.UpdateAsync(entity);
        }
    }
}