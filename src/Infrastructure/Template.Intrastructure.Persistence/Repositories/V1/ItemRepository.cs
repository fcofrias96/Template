using AutoMapper;
using Template.Application.Contracts.Repositories.V1;
using Template.Domain.Entities.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Repositories.V1
{
    public class ItemRepository : RepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context, IConfigurationProvider configurationProvider) : base(context, configurationProvider)
        {
        }
    }
}