using Template.Application.Contracts.Repositories.Base;
using Template.Domain.Entities.V1;
using Template.Security.DbContexts.V1;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IResourceRepository : IBaseRepository<Resource>
    {
    }
}