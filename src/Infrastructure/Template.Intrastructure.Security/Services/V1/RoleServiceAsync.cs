using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Models.Wrappers;
using Template.Application.Services.Base;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class RoleServiceAsync : ServiceBaseAsync<Role, SecurityDbContext>, IRoleServiceAsync
    {
        public RoleServiceAsync(
                ICreateServiceAsync<Role, SecurityDbContext> createService,
                IDeleteServiceAsync<Role, SecurityDbContext> deleteService,
                IExportServiceAsync<Role, SecurityDbContext> exportService,
                IReadServiceAsync<Role, SecurityDbContext> readService,
                IUpdateServiceAsync<Role, SecurityDbContext> updateService
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