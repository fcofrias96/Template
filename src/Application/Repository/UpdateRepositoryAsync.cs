using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class UpdateRepositoryAsync<TEntity> : IUpdateRepositoryAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly DbContext _dbContext;

        public UpdateRepositoryAsync(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

    }
}