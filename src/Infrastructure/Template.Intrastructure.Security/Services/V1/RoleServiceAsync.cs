using Microsoft.AspNetCore.Mvc;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.Wrappers;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class RoleServiceAsync : ServiceBaseAsync<Role, SecurityDbContext>, IRoleServiceAsync
    {
            private readonly IRolePermissionRepositoryAsync _rolePermissionRepository;

        public RoleServiceAsync(
                ICreateServiceAsync<Role, SecurityDbContext> createService,
                IDeleteServiceAsync<Role, SecurityDbContext> deleteService,
                IExportServiceAsync<Role, SecurityDbContext> exportService,
                IReadServiceAsync<Role, SecurityDbContext> readService,
                IUpdateServiceAsync<Role, SecurityDbContext> updateService,
                 IRolePermissionRepositoryAsync rolePermissionRepository
                ) :
            base(createService, deleteService, exportService, readService, updateService)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public override Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            string idString = id?.ToString() ?? string.Empty;
            return base.GetByIdProjectedAsync<string, TDestination>(idString, cancellationToken );
        }


        public override  async Task UpdateAsync<TKey, TSource>(TKey id, TSource source, CancellationToken cancellationToken)
        {
            if (source is RoleUpdateDto roleUpdateDTO && id is string _id)
            {
                await _rolePermissionRepository.UpdateAsync(_id, roleUpdateDTO.Permissions, cancellationToken);
            }
            await base.UpdateAsync(id, source, cancellationToken);
        }
    }

}