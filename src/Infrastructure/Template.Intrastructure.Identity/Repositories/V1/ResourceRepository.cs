using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class ResourceRepository : RepositoryBaseAsync<Resource>, IResourceRepositoryAsync
    {
        public ResourceRepository(
            ICreateRepositoryAsync<Resource> createRepository,
            IUpdateRepositoryAsync<Resource> updateRepository,
            IDeleteRepositoryAsync<Resource> deleteRepository,
            IReadRepositoryAsync<Resource> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}