using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Services.Base;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class ResourceService : ServiceBaseAsync<Resource>, IResourceServiceAsync
    {
        public ResourceService(
            ICreateServiceAsync<Resource> createService,
            IDeleteServiceAsync<Resource> deleteService,
            IExportServiceAsync<Resource> exportService,
            IReadServiceAsync<Resource> readService,
            IUpdateServiceAsync<Resource> updateService
            ) : base(createService, deleteService, exportService, readService, updateService)
        {
        }
    }
}