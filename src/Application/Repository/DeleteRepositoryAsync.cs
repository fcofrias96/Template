using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class DeleteRepositoryAsync<TEntity> :  IDeleteRepositoryAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly DbContext _dbContext;

        public DeleteRepositoryAsync(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

    }
}