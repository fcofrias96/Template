using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class RoleRepositoryAsync : RepositoryBaseAsync<Role,  IdentityContext>, IRoleRepositoryAsync
    {
        public RoleRepositoryAsync(
            ICreateRepositoryAsync<Role,  IdentityContext> createRepository,
            IUpdateRepositoryAsync<Role,  IdentityContext> updateRepository,
            IDeleteRepositoryAsync<Role,  IdentityContext> deleteRepository,
            IReadRepositoryAsync<Role,  IdentityContext> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}