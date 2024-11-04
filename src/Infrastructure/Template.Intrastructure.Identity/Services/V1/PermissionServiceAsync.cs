using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class PermissionServiceAsync : ServiceBaseAsync<Permission, IdentityContext>, IPermissionServiceAsync
    {
        private readonly IPermissionRepositoryAsync _permissionRepository;

        public PermissionServiceAsync(
                ICreateServiceAsync<Permission, IdentityContext> createService,
                IDeleteServiceAsync<Permission, IdentityContext> deleteService,
                IExportServiceAsync<Permission, IdentityContext> exportService,
                IReadServiceAsync<Permission, IdentityContext> readService,
                IUpdateServiceAsync<Permission, IdentityContext> updateService,
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