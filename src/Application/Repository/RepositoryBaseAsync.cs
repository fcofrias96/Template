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

        public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
          await _createRepository.CreateAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
           await _deleteRepository.DeleteAsync(entity, cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            return await _readRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            return await _readRepository.GetByIdProjectedAsync<TKey, TDestination>(id, cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
        {
           return await _readRepository.GetProjectedAsync<TDestination>(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            return await _readRepository.GetProjectedForExportAsync<TDestination>(spec, cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            return await _readRepository.GetProjectedPaginatedAsync<TDestination>(spec, cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria, CancellationToken cancellationToken)
        {
            return await _readRepository.GetTotalCountAsync(criteria, cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
           await _updateRepository.UpdateAsync(entity, cancellationToken);
        }
    }
}