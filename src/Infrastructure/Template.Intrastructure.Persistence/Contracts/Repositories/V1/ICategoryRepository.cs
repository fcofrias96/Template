using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Entities.V1;
using Template.Persistence.DbContexts.V1;

namespace Template.Persistence.Contracts.Repositories.V1
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
    }
}