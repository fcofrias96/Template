using Template.Application.Contracts.Services.Base;
using Template.Domain.Entities.V1;
using Template.Security.DbContexts.V1;

namespace Template.Security.Contracts.Services.V1
{
    public interface IResourceService : IBaseService<Resource>
    {
    }
}