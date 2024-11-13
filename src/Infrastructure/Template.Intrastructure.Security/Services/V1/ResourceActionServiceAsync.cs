using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class ResourceActionServiceAsync : ServiceBaseAsync<ResourceAction, SecurityDbContext>, IPermissionServiceAsync
    {
        private readonly IPermissionRepositoryAsync _permissionRepository;

        public ResourceActionServiceAsync(
                ICreateServiceAsync<ResourceAction, SecurityDbContext> createService,
                IDeleteServiceAsync<ResourceAction, SecurityDbContext> deleteService,
                IExportServiceAsync<ResourceAction, SecurityDbContext> exportService,
                IReadServiceAsync<ResourceAction, SecurityDbContext> readService,
                IUpdateServiceAsync<ResourceAction, SecurityDbContext> updateService,
                IPermissionRepositoryAsync permissionRepository

                ) : base(createService, deleteService, exportService, readService, updateService)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId)
        {
            return await _permissionRepository.HasPermissionAsync(userId, resourceId, actionId);
        }

    }
}