using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Application.Extensions;
using Template.Core.Domain.Contracts;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Persistence.DbContexts;

namespace Template.Infrastructure.Persistence.Repositories
{
    public class GenericRepositoryAsync<TEntity>: IGenericRepositoryAsync<TEntity> where TEntity: class, IAuditableEntity
    {
        #region Private Varible
        private readonly ApplicationDbContext _context;
        private readonly IConfigurationProvider _mapperConfiguration;
        #endregion Private Varible

        #region Constructor
        public GenericRepositoryAsync(ApplicationDbContext context, IConfigurationProvider mapperConfiguration) 
        { 
            _context = context;
            _mapperConfiguration = mapperConfiguration;
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



        public async Task<List<TDestination>> GetAllAsync<TDestination>(List<string>? navigationPropertyToInclude = null)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            if (navigationPropertyToInclude != null)
            {
                foreach (var NavigationProperty in navigationPropertyToInclude)
                {
                    query = query.Include(NavigationProperty);
                }
            }

            return await query.ProjectTo<TDestination>(_mapperConfiguration).ToListAsync();
        }

        public async Task<List<TDestination>> GetAllWithPaginationAsync<TDestination>(QueryParameters<TDestination> queryParameters, List<string>? navigationPropertyToInclude = default)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            if (navigationPropertyToInclude != null)
            {
                foreach (var NavigationProperty in navigationPropertyToInclude)
                {
                    query = query.Include(NavigationProperty);
                }
            }

            if(queryParameters.SortableProperties.Count > 0)
            {
                foreach(string property in queryParameters.SortableProperties)
                {
                    query = query.OrderByProperty(property, queryParameters.Descending);
                }
            }

            if(queryParameters.Filter != null)
            {
                query = query.SearchInStringProperties(queryParameters.Filter);
            }


            return await query
                        .Skip(queryParameters.Offset)
                        .Take(queryParameters.Limit)
                        .ProjectTo<TDestination>(_mapperConfiguration)
                        .ToListAsync();
        }
        public async Task<TEntity> GetByIdAsync<TKey>(TKey id, List<string>?  navigationPropertyToInclude = default)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            if(navigationPropertyToInclude != null)
            {
                foreach (var NavigationProperty in navigationPropertyToInclude)
                {
                    query = query.Include(NavigationProperty);
                }
            }

            return await query.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }
            
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        #endregion Public Methods

    }
}
