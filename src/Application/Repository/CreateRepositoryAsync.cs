using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class CreateRepositoryAsync<TEntity, TDbContext> : ICreateRepositoryAsync<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public CreateRepositoryAsync(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}