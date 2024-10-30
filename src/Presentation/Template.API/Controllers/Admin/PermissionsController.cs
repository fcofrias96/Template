using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = Roles.Admin)]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionServiceAsync _permissionService;

        public PermissionsController(IPermissionServiceAsync permissionService)
        {
            _permissionService = permissionService;
        }

        [Permission(Resources.Permission, Actions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PermissionListDto>>>> GetPermissions()
        {
            var permissions = await _permissionService.GetProjectedAsync<PermissionListDto>();
            return Ok(permissions);
        }

        [Permission(Resources.Permission, Actions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PermissionDetailDto>>> GetPermission(int id)
        {
            var permission = await _permissionService.GetByIdProjectedAsync<int, PermissionDetailDto>(id);
            return Ok(permission);
        }

        [Permission(Resources.Permission, Actions.Create)]
        [HttpPost]
        public async Task<ActionResult> CreatePermission([FromBody] PermissionCreateDto permissionCreateDto)
        {
            var permission = await _permissionService.CreateAsync(permissionCreateDto);

            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
        }

        [Permission(Resources.Permission, Actions.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePermission(int id, [FromBody] PermissionUpdateDTO permissionUpdateDTO)
        {
            await _permissionService.UpdateAsync(id, permissionUpdateDTO);
            return NoContent();
        }

        [Permission(Resources.Permission, Actions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePermission(int id)
        {
            await _permissionService.SoftDeleteAsync(id);
            return NoContent();
        }
    }
}