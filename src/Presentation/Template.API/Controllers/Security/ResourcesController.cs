
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Resources;
using Template.Security.Specification.V1;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize(Roles = Roles.Admin)]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceServiceAsync _resourceService;

        public ResourcesController(IResourceServiceAsync resourceService)
        {
            _resourceService = resourceService;
        }

        [Permission(Resources.Resource, ActionType.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ResourceListDto>>>> GetProjectedPaginatedAsync([FromQuery] PermissionQueryParameters request, CancellationToken cancellationToken)
        {
            var resources = await _resourceService.GetProjectedPaginatedAsync<ResourceListDto, ResourceSpecification>(request, cancellationToken);
            return Ok(resources);
        }

        [Permission(Resources.Resource, ActionType.Read)]
        [HttpGet("dropdown")]
        public async Task<ActionResult<ApiResponse<List<Dropdown>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
            var resources = await _resourceService.GetProjectedAsync<Dropdown>(cancellationToken);
            return Ok(resources);
        }

        [Permission(Resources.Resource, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ResourceDetailDto>>> GetResource(int id, CancellationToken cancellationToken)
        {
            var resource = await _resourceService.GetByIdProjectedAsync<int, ResourceDetailDto>(id, cancellationToken);
            return Ok(resource);
        }

        [Permission(Resources.Resource, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateResource([FromBody] ResourceCreateDto resourceCreateDto, CancellationToken cancellationToken)
        {
            var resource = await _resourceService.CreateAsync(resourceCreateDto, cancellationToken);

            return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
        }

        [Permission(Resources.Resource, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateResource(int id, [FromBody] ResourceUpdateDto resourceUpdateDTO, CancellationToken cancellationToken)
        {
            await _resourceService.UpdateAsync(id, resourceUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Resource, ActionType.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResource(int id, CancellationToken cancellationToken)
        {
            await _resourceService.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}