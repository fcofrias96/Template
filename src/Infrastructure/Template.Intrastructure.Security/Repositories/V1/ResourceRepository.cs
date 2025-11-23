using AutoMapper;
using Template.Application.Repository;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Repositories.V1
{
    public class ResourceRepository : BaseRepositoryAsync<Resource>, IResourceRepositoryAsync
    {
        public ResourceRepository(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
        }
    }
}