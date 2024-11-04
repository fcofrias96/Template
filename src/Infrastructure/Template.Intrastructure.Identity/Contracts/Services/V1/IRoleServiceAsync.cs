using Template.Application.Contracts.Services.Base;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IRoleServiceAsync : IAsyncServiceAsync<Role, IdentityContext>
    {
    }
}