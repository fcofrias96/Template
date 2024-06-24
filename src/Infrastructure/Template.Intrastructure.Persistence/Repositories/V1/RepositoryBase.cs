using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.V1;
using Template.Domain.Contracts;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Repositories.V1
{
    public class RepositoryBase<TEntity> : IAsyncRepository<TEntity> where TEntity : class, IEntityBase
    {
        #region Private Variable

        private readonly IConfigurationProvider _configurationProvider;
        private readonly ApplicationDbContext _dbContext;
        #endregion Private Variable

        public RepositoryBase(ApplicationDbContext context, IConfigurationProvider configurationProvider)
        {
            _dbContext = context;
            _configurationProvider = configurationProvider;
        }

        #region Public Methods

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TDestination>> GetAllProjectedAsync<TDestination>()
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync();
        }

        public async Task<List<TDestination>> GetAllProjectedWithPaginationAsync<TDestination>(ISpecification<TEntity> spec)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();
            query = ApplySpecification(query, spec);
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync();
        }

        public async Task<IList<TDestination>> GetAllProjectedToExportAsync<TDestination>(ISpecification<TEntity> spec)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();
            query = ApplySpecification(query, spec, applyPagination: false );
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync();
        }


        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();
            return await query.AsNoTracking()
                              .Where(x => x.Id.Equals(id))
                              .ProjectTo<TDestination>(_configurationProvider)
                              .FirstOrDefaultAsync();
        }
        public async Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria)
        {
            if (criteria == null)
            {
                return await _dbContext.Set<TEntity>().CountAsync();
            }
            else
            {
                return await _dbContext.Set<TEntity>().Where(criteria).CountAsync();
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion Public Methods

        #region Private Methods



        private IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> spec, bool applyPagination = true)
        {
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.Descending)
            {
                query = query.OrderByDescending(spec.SortBy);
            }
            else
            {
                query = query.OrderBy(spec.SortBy);
            }

            if (applyPagination)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
           

            return query;
        }
        #endregion Private Methods
    }
}