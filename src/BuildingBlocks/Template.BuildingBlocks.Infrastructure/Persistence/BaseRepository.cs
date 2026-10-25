using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Repositories.Base;
using Template.SharedKernel.Domain;

namespace Template.BuildingBlocks.Infrastructure.Persistence
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class, IEntityBase
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await _dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task HardDeletesAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(projection);

            return await _dbSet
                .AsNoTracking()
                .Select(projection)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedAsync<TDestination>(
            ISpecification<TEntity> spec,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(spec);
            ArgumentNullException.ThrowIfNull(projection);

            var query = _dbSet
                .AsQueryable()
                .AsNoTracking();

            query = ApplySpecification(query, spec, applyPagination: false);

            return await query
                .Select(projection)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedPaginatedAsync<TDestination>(
            ISpecification<TEntity> spec,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(spec);
            ArgumentNullException.ThrowIfNull(projection);

            var query = _dbSet
                .AsQueryable()
                .AsNoTracking();

            query = ApplySpecification(query, spec);

            return await query
                .Select(projection)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TDestination>> GetProjectedForExportAsync<TDestination>(
            ISpecification<TEntity> spec,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(spec);
            ArgumentNullException.ThrowIfNull(projection);

            var query = _dbSet
                .AsQueryable()
                .AsNoTracking();

            query = ApplySpecification(query, spec, applyPagination: false);

            return await query
                .Select(projection)
                .ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
        }

        public async Task<TDestination?> GetByIdProjectedAsync<TDestination>(
            int id,
            Expression<Func<TEntity, TDestination>> projection,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(projection);

            return await _dbSet
                .AsNoTracking()
                .Where(entity => entity.Id == id)
                .Select(projection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(spec);

            var query = _dbSet.AsQueryable();

            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<TEntity> ApplySpecification(
            IQueryable<TEntity> query,
            ISpecification<TEntity> spec,
            bool applyPagination = true,
            bool applySorting = true)
        {
            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            }

            if (applySorting && spec.SortBy is not null)
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

        public async Task<(DateTimeOffset LastModifiedAt, int Count)> GetListMetadataAsync(
            ISpecification<TEntity>? spec = default,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            if (spec is not null)
            {
                query = ApplySpecification(query, spec, applyPagination: false);
            }

            var lastModifiedAt = await query.MaxAsync(entity => entity.LastModifiedAt, cancellationToken)
                ?? DateTimeOffset.MinValue;

            var count = await query.CountAsync(cancellationToken);

            return (lastModifiedAt, count);
        }

        public async Task<byte[]?> GetRowVersionAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(entity => entity.Id == id)
                .Select(entity => entity.RowVersion)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            entity.MarkDeleted();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
