using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class RoleRepositoryAsync : RepositoryBaseAsync<Role, SecurityDbContext>, IRoleRepositoryAsync
    {
        public RoleRepositoryAsync(
            ICreateRepositoryAsync<Role, SecurityDbContext> createRepository,
            IUpdateRepositoryAsync<Role, SecurityDbContext> updateRepository,
            IDeleteRepositoryAsync<Role, SecurityDbContext> deleteRepository,
            IReadRepositoryAsync<Role, SecurityDbContext> readRepository

            ) : base(createRepository, updateRepository, deleteRepository, readRepository)
        {
        }


    }
}