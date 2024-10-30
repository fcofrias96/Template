using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class RoleRepositoryAsync : RepositoryBaseAsync<Role>, IRoleRepositoryAsync
    {
        public RoleRepositoryAsync(
            ICreateRepositoryAsync<Role> createRepository,
            IUpdateRepositoryAsync<Role> updateRepository,
            IDeleteRepositoryAsync<Role> deleteRepository,
            IReadRepositoryAsync<Role> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }
    }
}