using System.Linq.Expressions;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Repositories.V1;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class RepositoryBaseAsync<TEntity> : IAsyncRepository<TEntity> where TEntity : class, IEntityBase
    {
        #region Private Variable

        private readonly ICreateRepositoryAsync<TEntity> _createRepository;
        private readonly IDeleteRepositoryAsync<TEntity> _deleteRepository;
        private readonly IReadRepositoryAsync<TEntity> _readRepository;
        private readonly IUpdateRepositoryAsync<TEntity> _updateRepository;
        public RepositoryBaseAsync(
            ICreateRepositoryAsync<TEntity> createRepository, 
            IUpdateRepositoryAsync<TEntity> updateRepository, 
            IDeleteRepositoryAsync<TEntity> deleteRepository, 
            IReadRepositoryAsync<TEntity> readRepository
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