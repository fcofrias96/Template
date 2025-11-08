using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class UpdateRepositoryAsync<TEntity,TDbContext> : IUpdateRepositoryAsync<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public UpdateRepositoryAsync(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}