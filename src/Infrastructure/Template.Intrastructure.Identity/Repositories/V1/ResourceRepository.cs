using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class ResourceRepository : RepositoryBaseAsync<Resource,  IdentityContext>, IResourceRepositoryAsync
    {
        public ResourceRepository(
            ICreateRepositoryAsync<Resource,  IdentityContext> createRepository,
            IUpdateRepositoryAsync<Resource,  IdentityContext> updateRepository,
            IDeleteRepositoryAsync<Resource,  IdentityContext> deleteRepository,
            IReadRepositoryAsync<Resource,  IdentityContext> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}