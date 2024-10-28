using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Template.Application.Exceptions;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Services;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class PermissionServiceAsync :  ServiceBaseAsync<Permission>, IPermissionServiceAsync
    {
        private readonly IPermissionRepositoryAsync _permissionRepository;
        private readonly IResourceRepositoryAsync _resourceRepository;
        private readonly IActionRepositoryAsync _actionRepository;
        public PermissionServiceAsync(
            IPermissionRepositoryAsync repository,  
            IMapper mapper, 
            IHttpContextAccessor httpContext, 
            IMemoryCache cache, 
            IResourceRepositoryAsync resourceRepository,
            IActionRepositoryAsync actionRepository,
            ILogger logger

            ) : base(repository, mapper, httpContext, cache, logger)
        {
            _permissionRepository = repository;
            _resourceRepository = resourceRepository;
            _actionRepository = actionRepository;
        }

        public async Task<bool> HasPermissionAsync(string userId, int resourceId, int actionId)
        {
           return await _permissionRepository.HasPermissionAsync(userId, resourceId, actionId);
        }

        public async Task<Permission> CreateAsync(PermissionCreateDto permissionCreate)
        {
            var resource = await _resourceRepository.GetByIdAsync(permissionCreate.ResourceId);
            var action = await _actionRepository.GetByIdAsync(permissionCreate.ActionId);

            if (resource == null || action == null)
            {
                throw new BadRequestException("Invalid resource or action");
            }
            return await base.CreateAsync(permissionCreate);
        }
    }

}