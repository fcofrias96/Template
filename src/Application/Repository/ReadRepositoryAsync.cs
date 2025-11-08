using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Contracts;

namespace Template.Application.Repository
{
    public class ReadRepositoryAsync<TEntity, TDbContext> : IReadRepositoryAsync<TEntity, TDbContext> 
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly DbSet<TEntity> _dbSet;

        public ReadRepositoryAsync(TDbContext dbContext, IConfigurationProvider configurationProvider)
        {
            _dbSet = dbContext.Set<TEntity>();
            _configurationProvider = configurationProvider;
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            var query = _dbSet.AsQueryable();
            query = ApplySpecification(query, spec);
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            var query = _dbSet.AsQueryable();
            query = ApplySpecification(query, spec, applyPagination: false);
            return await query.AsNoTracking()
                              .ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(new object?[] { id, cancellationToken }, cancellationToken: cancellationToken);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            var query = _dbSet.AsQueryable();
            return await query.AsNoTracking()
                              .Where(x => x.Id.Equals(id))
                              .ProjectTo<TDestination>(_configurationProvider)
                              .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>>? criteria, CancellationToken cancellationToken)
        {
            if (criteria == null)
            {
                return await _dbSet.CountAsync(cancellationToken);
            }
            else
            {
                return await _dbSet.Where(criteria).CountAsync(cancellationToken);
            }
        }


        #region Private Methods

        private static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> spec, bool applyPagination = true)
        {
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.IsDescending)
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