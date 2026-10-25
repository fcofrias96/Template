using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.Modules.Catalog.Application.Contracts;

namespace Template.Modules.Catalog.Infrastructure
{
    public sealed class CatalogUnitOfWork : UnitOfWork<CatalogDbContext>, ICatalogUnitOfWork
    {
        public CatalogUnitOfWork(CatalogDbContext context) : base(context)
        {
        }
    }
}
