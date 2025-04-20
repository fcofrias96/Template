using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Permissions;
using Template.Security.Specification.V1;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize(Roles = Roles.Admin)]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionServiceAsync _permissionService;

        public PermissionsController(IPermissionServiceAsync permissionService)
        {
            _permissionService = permissionService;
        }

        [Permission(Resources.Permission, ActionType.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PermissionListDto>>>> GetPermissions([FromQuery] PermissionQueryParameters request)
        {
            var permissions = await _permissionService.GetProjectedPaginatedAsync<PermissionListDto, PermissionSpecification>(request);
            return Ok(permissions);
        }

        [Permission(Resources.Permission, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PermissionDetailDto>>> GetPermission(int id)
        {
            var permission = await _permissionService.GetByIdProjectedAsync<int, PermissionDetailDto>(id);
            return Ok(permission);
        }

        [Permission(Resources.Permission, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult> CreatePermission([FromBody] PermissionCreateDto permissionCreateDto)
        {
            var permission = await _permissionService.CreateAsync(permissionCreateDto);

            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
        }

        [Permission(Resources.Permission, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePermission(int id, [FromBody] PermissionUpdateDto permissionUpdateDTO)
        {
            await _permissionService.UpdateAsync(id, permissionUpdateDTO);
            return NoContent();
        }

        [Permission(Resources.Permission, ActionType.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePermission(int id)
        {
            await _permissionService.SoftDeleteAsync(id);
            return NoContent();
        }

        [Permission(Resources.Permission, ActionType.Read)]
        [HttpGet("dropdown")]
        public async Task<ActionResult<ApiResponse<List<Dropdown>>>> GetProjectedAsync()
        {
            var resources = await _permissionService.GetProjectedAsync<Dropdown>();
            return Ok(resources);
        }
    }
}