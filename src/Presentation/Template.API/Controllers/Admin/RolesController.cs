using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.Role;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = Roles.Admin)]
    public class RolesController : ControllerBase
    {

        private readonly IRoleServiceAsync _roleService;
        private readonly IRolePermissionServiceAsync _rolePermissionService;

        public RolesController(IRoleServiceAsync roleService, IRolePermissionServiceAsync rolePermissionService)
        {
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
        }

        [Permission(Resources.Role, Actions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoleListDto>>>> GetRoles()
        {
            var roles = await _roleService.GetAllProjectedAsync<RoleListDto>();
            return Ok(roles);
        }

        [Permission(Resources.Role, Actions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRole(Guid id)
        {
            var role = await _roleService.GetByIdProjectedAsync<Guid, RoleDetailDto>(id);
            return Ok(role);
        }

        [Permission(Resources.Role, Actions.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] RoleCreateDto roleCreateDto)
        {
            var role = await _roleService.CreateAsync(roleCreateDto);

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        [Permission(Resources.Role, Actions.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(Guid id, [FromBody] RoleUpdateDto roleUpdateDTO)
        {
            await _roleService.UpdateAsync(id, roleUpdateDTO);
            return NoContent();
        }

        [Permission(Resources.Role, Actions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(Guid id)
        {
            await _roleService.DeleteLogicalAsync(id);
            return NoContent();
        }

        [Permission(Resources.Role, Actions.Create)]
        [HttpPost("{id}/AddPermission")]
        public async Task<ActionResult> AddPermissionToRole(Guid id, [FromBody] int[] permissionsId)
        {
            await _rolePermissionService.AddPermissionToRoleAsync(id, permissionsId);
            return NoContent();
        }

        [Permission(Resources.Role, Actions.Delete)]
        [HttpPost("{id}/RemovePermission")]
        public async Task<ActionResult> RemovePermissionToRole(Guid id, [FromBody] int[] permissionsId)
        {
            await _rolePermissionService.RemovePermissionToRoleAsync(id, permissionsId);
            return NoContent();
        }
    }
}
