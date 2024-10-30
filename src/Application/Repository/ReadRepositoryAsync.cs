using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class ReadRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity> where TEntity : class, IEntityBase
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly DbSet<TEntity> _dbSet;

        public ReadRepositoryAsync(DbContext dbContext, IConfigurationProvider configurationProvider)
        {
            _dbSet = dbContext.Set<TEntity>();
            _configurationProvider = configurationProvider;
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>()
        {
            var query = _dbSet.AsQueryable();
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync();
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec)
        {
            var query = _dbSet.AsQueryable();
            query = ApplySpecification(query, spec);
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync();
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec)
        {
            var query = _dbSet.AsQueryable();
            query = ApplySpecification(query, spec, applyPagination: false);
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            var query = _dbSet.AsQueryable();
            return await query.AsNoTracking()
                              .Where(x => x.Id.Equals(id))
                              .ProjectTo<TDestination>(_configurationProvider)
                              .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria)
        {
            if (criteria == null)
            {
                return await _dbSet.CountAsync();
            }
            else
            {
                return await _dbSet.Where(criteria).CountAsync();
            }
        }


        #region Private Methods

        private static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> spec, bool applyPagination = true)
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