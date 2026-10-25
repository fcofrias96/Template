using Template.BuildingBlocks.Application.Contracts.Repositories.Base;
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Application.Contracts.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
    }
}
