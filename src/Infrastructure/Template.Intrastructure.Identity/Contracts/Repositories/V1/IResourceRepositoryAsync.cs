using Template.Application.Contracts.Repositories.V1;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IResourceRepositoryAsync : IAsyncRepository<Resource>
    {
    }
}