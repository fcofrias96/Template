using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class ResourceService : ServiceBaseAsync<Resource, IdentityContext>, IResourceServiceAsync
    {
        public ResourceService(
            ICreateServiceAsync<Resource, IdentityContext> createService,
            IDeleteServiceAsync<Resource, IdentityContext> deleteService,
            IExportServiceAsync<Resource, IdentityContext> exportService,
            IReadServiceAsync<Resource, IdentityContext> readService,
            IUpdateServiceAsync<Resource, IdentityContext> updateService
            ) : base(createService, deleteService, exportService, readService, updateService)
        {
        }
    }
}