using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Api.Controllers.Common;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.Specification;
using Template.Application.Models.Wrappers;
using Template.Domain.Entities.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Resources;


namespace Template.Api.Controllers.Security
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ResourcesController :  ApiControllerBase
    {
        private readonly IResourceService _resourceService;

        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [Permission(Resource.ResourceItem, PermissionAction.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ResourceListDto>>>> GetProjectedPaginatedAsync([FromQuery] PermissionQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Resource>(request);
            var resources = await _resourceService.GetProjectedPaginatedAsync<ResourceListDto>(specification, cancellationToken);
            return Ok(resources);
        }

        [Permission(Resource.ResourceItem, PermissionAction.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<Catalog>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
			var result = await _resourceService.GetProjectedWithEtagAsync<Catalog>(cancellationToken);
			return FromEtagResult(result);
		}

        [Permission(Resource.ResourceItem, PermissionAction.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ResourceDetailDto>>> GetResource(int id, CancellationToken cancellationToken)
        {
            var resource = await _resourceService.GetByIdProjectedAsync<int, ResourceDetailDto>(id, cancellationToken);
            return Ok(resource);
        }

        [Permission(Resource.ResourceItem, PermissionAction.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateResource([FromBody] ResourceCreateDto resourceCreateDto, CancellationToken cancellationToken)
        {
            var resource = await _resourceService.CreateAsync(resourceCreateDto, cancellationToken);

            return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
        }

        [Permission(Resource.ResourceItem, PermissionAction.Update)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateResource(int id, [FromBody] ResourceUpdateDto resourceUpdateDTO, CancellationToken cancellationToken)
        {
            await _resourceService.UpdateAsync(id, resourceUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(Resource.ResourceItem, PermissionAction.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResource(int id, CancellationToken cancellationToken)
        {
            await _resourceService.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}