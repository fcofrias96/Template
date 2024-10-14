using AutoMapper;
using Template.Application.Repository;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContext.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class ResourceRepository : RepositoryBaseAsync<Resource>, IResourceRepositoryAsync
    {
        public ResourceRepository(IdentityContext context, IConfigurationProvider configurationProvider) : base(context, configurationProvider)
        {
        }
    }
}