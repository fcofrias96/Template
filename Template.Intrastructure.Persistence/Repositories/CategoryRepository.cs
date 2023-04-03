using AutoMapper;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Persistence.DbContexts;

namespace Template.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepositoryAsync<Category>, ICategoryRepositoryAsync
    {
        #region Constructor

        public CategoryRepository(ApplicationDbContext context, IConfigurationProvider configurationProvider) : base(context, configurationProvider)
        {
        }

        #endregion Constructor
    }
}