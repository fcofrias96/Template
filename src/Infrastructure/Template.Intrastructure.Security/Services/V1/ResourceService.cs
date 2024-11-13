using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class ResourceService : ServiceBaseAsync<Resource, SecurityDbContext>, IResourceServiceAsync
    {
        public ResourceService(
            ICreateServiceAsync<Resource, SecurityDbContext> createService,
            IDeleteServiceAsync<Resource, SecurityDbContext> deleteService,
            IExportServiceAsync<Resource, SecurityDbContext> exportService,
            IReadServiceAsync<Resource, SecurityDbContext> readService,
            IUpdateServiceAsync<Resource, SecurityDbContext> updateService
            ) : base(createService, deleteService, exportService, readService, updateService)
        {
        }
    }
}