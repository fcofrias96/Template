using AutoMapper;

using Template.Application.Repository;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Repositories.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Repositories.V1
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
        }
    }
}