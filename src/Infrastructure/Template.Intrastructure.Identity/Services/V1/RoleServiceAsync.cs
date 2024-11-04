using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Models.Wrappers;
using Template.Application.Services.Base;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class RoleServiceAsync : ServiceBaseAsync<Role, IdentityContext>, IRoleServiceAsync
    {
        public RoleServiceAsync(
                ICreateServiceAsync<Role, IdentityContext> createService,
                IDeleteServiceAsync<Role, IdentityContext> deleteService,
                IExportServiceAsync<Role, IdentityContext> exportService,
                IReadServiceAsync<Role, IdentityContext> readService,
                IUpdateServiceAsync<Role, IdentityContext> updateService
                ) :
            base(createService, deleteService, exportService, readService, updateService)
        {
        }

        public override Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            string idString = id?.ToString() ?? string.Empty;
            return base.GetByIdProjectedAsync<string, TDestination>(idString);
        }
    }
}