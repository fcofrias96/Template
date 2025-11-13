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
            var query = _dbSet.AsQueryable().AsNoTracking();
            query = ApplySpecification(query, spec);
            
            return await query.ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(ISpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            var query = _dbSet.AsQueryable().AsNoTracking();
            query = ApplySpecification(query, spec, applyPagination: false);
            
            return await query.ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(new object?[] { id, cancellationToken }, cancellationToken: cancellationToken);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            var query = _dbSet.AsQueryable().AsNoTracking();

            return await query.Where(x => x.Id.Equals(id))
                              .ProjectTo<TDestination>(_configurationProvider)
                              .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            var query = _dbSet.AsQueryable();

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            return await query.CountAsync(cancellationToken);
        }


        #region Private Methods

        private static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> spec, bool applyPagination = true, bool applySorting = true)
        {
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (applySorting)
            {
                query = spec.IsDescending
                    ? query.OrderByDescending(spec.SortBy)
                    : query.OrderBy(spec.SortBy);
            }

            if (applyPagination && spec.Take > 0)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }

        #endregion Private Methods

    }
}