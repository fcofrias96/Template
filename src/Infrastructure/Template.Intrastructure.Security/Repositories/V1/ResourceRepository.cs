using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class ResourceRepository : RepositoryBaseAsync<Resource, SecurityDbContext>, IResourceRepositoryAsync
    {
        public ResourceRepository(
            ICreateRepositoryAsync<Resource, SecurityDbContext> createRepository,
            IUpdateRepositoryAsync<Resource, SecurityDbContext> updateRepository,
            IDeleteRepositoryAsync<Resource, SecurityDbContext> deleteRepository,
            IReadRepositoryAsync<Resource, SecurityDbContext> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}