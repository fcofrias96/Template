using Template.Application.Services.Base;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class ResourceService : BaseService<Resource>, IResourceService
    {
        public ResourceService(BaseServiceDeps<Resource> deps) : base(deps)
        {
        }
    }
}