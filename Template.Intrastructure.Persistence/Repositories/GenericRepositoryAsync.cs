using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Core.Application.Contracts;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Domain.Contracts;
using Template.Infrastructure.Persistence.DbContexts;

namespace Template.Infrastructure.Persistence.Repositories
{
    public class GenericRepositoryAsync<TEntity> : IGenericRepositoryAsync<TEntity> where TEntity : class, IAuditableEntity
    {
        #region Private Variable

        private readonly ApplicationDbContext _context;
        private readonly IConfigurationProvider _configurationProvider;

        #endregion Private Variable

        #region Constructor

        public GenericRepositoryAsync(ApplicationDbContext context, IConfigurationProvider configurationProvider)
        {
            _context = context;
            _configurationProvider = configurationProvider;
        }

        #endregion Constructor

        #region Public MethodsT


        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<TDestination?> GetByIdAsync<TKey, TDestination>(TKey id)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            return await query.Where(x => x.Id.Equals(id))
                              .ProjectTo<TDestination>(_configurationProvider)
                              .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalCountAsync(Expression<Func<TEntity, bool>> criteria)
        {
            if(criteria == null)
            {
                return await _context.Set<TEntity>().CountAsync();
            }
            else
            {
                return await _context.Set<TEntity>().Where(criteria).CountAsync();
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<List<TDestination>> GetAllAsync<TDestination>(ISpecification<TEntity> spec)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (spec != null)
            {
                query = ApplySpecification(query, spec);
            }

            string x = query.ProjectTo<TDestination>(_configurationProvider).ToQueryString();

            return await query.ProjectTo<TDestination>(_configurationProvider)
                              .ToListAsync();
        }


        #endregion Public Methods

        #region Private Methods
        private IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> spec)
        {
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.Descending)
            {
                query = query.OrderByDescending(spec.OrderBy);
            }
            else
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }
        #endregion Private Methods
    }
}