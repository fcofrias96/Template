using AutoMapper;
using Template.Application.Repository;
using Template.Domain.Entities.V1;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.DbContexts.V1;

namespace Template.Security.Repositories.V1
{
    public class ResourceRepository : BaseRepository<Resource>, IResourceRepository
    {
        public ResourceRepository(SecurityDbContext securityDb, IConfigurationProvider configurationProvider) : base(securityDb, configurationProvider)
        {
        }
    }
}