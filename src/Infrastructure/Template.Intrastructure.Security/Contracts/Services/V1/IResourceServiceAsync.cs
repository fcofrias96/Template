using Template.Application.Contracts.Services.Base;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IResourceServiceAsync : IBaseServiceAsync<Resource, SecurityDbContext>
    {
    }
}