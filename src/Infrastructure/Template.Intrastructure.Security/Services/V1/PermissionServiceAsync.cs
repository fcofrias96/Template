using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class PermissionServiceAsync : ServiceBaseAsync<Permission, SecurityDbContext>, IPermissionServiceAsync
    {
        private readonly IPermissionRepositoryAsync _permissionRepository;

        public PermissionServiceAsync(
                ICreateServiceAsync<Permission, SecurityDbContext> createService,
                IDeleteServiceAsync<Permission, SecurityDbContext> deleteService,
                IExportServiceAsync<Permission, SecurityDbContext> exportService,
                IReadServiceAsync<Permission, SecurityDbContext> readService,
                IUpdateServiceAsync<Permission, SecurityDbContext> updateService,
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