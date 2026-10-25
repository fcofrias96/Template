using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Contracts.Authorization;
using Template.BuildingBlocks.Web.Extensions;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Application.Dtos.Lookups;
using Template.Modules.Identity.Application.Dtos.Resources;
using Template.BuildingBlocks.Application.Models.Specification;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Domain.Entities;


namespace Template.Modules.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceService _resourceService;

        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [Permission(PermissionResources.ResourceItem, PermissionActions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ResourceListDto>>>> GetProjectedPaginatedAsync([FromQuery] ResourceQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Resource>(request);
            var resources = await _resourceService.GetProjectedPaginatedAsync<ResourceListDto>(specification, cancellationToken);
            return Ok(resources);
        }

        [Permission(PermissionResources.ResourceItem, PermissionActions.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
			var result = await _resourceService.GetProjectedWithEtagAsync<ResourceLookupDto>(cancellationToken);
			return result.ToActionResult();
		}

        [Permission(PermissionResources.ResourceItem, PermissionActions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ResourceDetailDto>>> GetResource(int id, CancellationToken cancellationToken)
        {
            var resource = await _resourceService.GetByIdProjectedAsync<ResourceDetailDto>(id, cancellationToken);
            return Ok(resource);
        }

        [Permission(PermissionResources.ResourceItem, PermissionActions.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateResource([FromBody] ResourceCreateDto resourceCreateDto, CancellationToken cancellationToken)
        {
            var resource = await _resourceService.CreateAsync(resourceCreateDto, cancellationToken);

            return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
        }

        [Permission(PermissionResources.ResourceItem, PermissionActions.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateResource(int id, [FromBody] ResourceUpdateDto resourceUpdateDTO, CancellationToken cancellationToken)
        {
            await _resourceService.UpdateAsync(id, resourceUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(PermissionResources.ResourceItem, PermissionActions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResource(int id, CancellationToken cancellationToken)
        {
            await _resourceService.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
