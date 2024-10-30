using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class CreateRepositoryAsync<TEntity> : ICreateRepositoryAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly DbContext _dbContext;

        public CreateRepositoryAsync(DbContext dbContext)
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