using Template.Application.Contracts.Repositories.Base;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IResourceRepositoryAsync : IBaseRepositoryAsync<Resource, SecurityDbContext>
    {
    }
}