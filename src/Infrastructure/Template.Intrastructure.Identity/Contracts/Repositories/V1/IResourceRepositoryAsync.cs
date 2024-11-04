using Template.Application.Contracts.Repositories.Base;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IResourceRepositoryAsync : IAsyncRepository<Resource, IdentityContext>
    {
    }
}