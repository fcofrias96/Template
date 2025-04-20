using Template.Security.Entities;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;

namespace Template.Security.Services.V1
{
    public class RolePermissionServiceAsync : IRolePermissionServiceAsync
    {
        private readonly IRolePermissionRepositoryAsync _rolePermissionRepositoryAsync;

        public RolePermissionServiceAsync(IRolePermissionRepositoryAsync rolePermissionRepositoryAsync)
        {
            _rolePermissionRepositoryAsync = rolePermissionRepositoryAsync;
        }



    }
}