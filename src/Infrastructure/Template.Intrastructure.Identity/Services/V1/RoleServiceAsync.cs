using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Models.Wrappers;
using Template.Application.Services.Base;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class RoleServiceAsync : ServiceBaseAsync<Role>, IRoleServiceAsync
    {
        public RoleServiceAsync(
                ICreateServiceAsync<Role> createService,
                IDeleteServiceAsync<Role> deleteService,
                IExportServiceAsync<Role> exportService,
                IReadServiceAsync<Role> readService,
                IUpdateServiceAsync<Role> updateService
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