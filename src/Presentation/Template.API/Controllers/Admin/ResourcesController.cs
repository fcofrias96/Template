using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = Roles.Admin)]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceServiceAsync _resourceService;

        public ResourcesController(IResourceServiceAsync resourceService)
        {
            _resourceService = resourceService;
        }

        [Permission(Resources.Resource, Actions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ResourceListDto>>>> GetResources()
        {
            var resources = await _resourceService.GetProjectedAsync<ResourceListDto>();
            return Ok(resources);
        }

        [Permission(Resources.Resource, Actions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ResourceDetailDto>>> GetResource(int id)
        {
            var resource = await _resourceService.GetByIdProjectedAsync<int, ResourceDetailDto>(id);
            return Ok(resource);
        }

        [Permission(Resources.Resource, Actions.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateResource([FromBody] ResourceCreateDto resourceCreateDto)
        {
            var resource = await _resourceService.CreateAsync(resourceCreateDto);

            return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
        }

        [Permission(Resources.Resource, Actions.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateResource(int id, [FromBody] ResourceUpdateDto resourceUpdateDTO)
        {
            await _resourceService.UpdateAsync(id, resourceUpdateDTO);
            return NoContent();
        }

        [Permission(Resources.Resource, Actions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResource(int id)
        {
            await _resourceService.SoftDeleteAsync(id);
            return NoContent();
        }
    }
}