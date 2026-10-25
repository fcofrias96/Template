
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Catalog.Domain.Entities;
using Template.Modules.Catalog.Application.Contracts.Repositories;
using Template.Modules.Catalog.Infrastructure;

namespace Template.Modules.Catalog.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(CatalogDbContext securityDb) : base(securityDb)
        {
        }
    }
}
