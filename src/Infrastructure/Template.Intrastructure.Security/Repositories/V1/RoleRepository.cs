using AutoMapper;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
        }
    }
}