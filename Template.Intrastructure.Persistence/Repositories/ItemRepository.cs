using AutoMapper;
using Template.Core.Application.Contracts.Repositories;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Persistence.DbContexts;

namespace Template.Infrastructure.Persistence.Repositories
{
    public class ItemRepository : GenericRepositoryAsync<Item>, IItemRepositoryAsync
    {
        #region Constructor

        public ItemRepository(ApplicationDbContext context, IConfigurationProvider configurationProvider) : base(context, configurationProvider)
        {
        }

        #endregion Constructor
    }
}