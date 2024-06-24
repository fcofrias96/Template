using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Module;
using Template.Application.Models.DTOs.V1.ModulePermission;
using Template.Application.Models.DTOs.V1.Role;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Specification.V1;

namespace Template.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        #region Private Variable

        private readonly IUserServiceAsync _userServiceAsync;

        #endregion Private Variable

        public UsersController(IUserServiceAsync userService)
        {
            _userServiceAsync = userService;
        }

        #region Actions

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{id}/Modules")]
        public async Task<ActionResult> AddModulesAsync(Guid id, [FromBody] List<ModulePermissionAddDTO> modulePermissions)
        {
            await _userServiceAsync.AddModulesAsync(id, modulePermissions);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{id}/Modules/{moduleId}/permissions")]
        public async Task<ActionResult> AddPermissionsFromModuleAsync(Guid id, int moduleId, [FromBody] ModulePermissionIdsDTO modulePermissionIds)
        {
            await _userServiceAsync.AddPermissionsFromModuleAsync(id, moduleId, modulePermissionIds.PermissionIds);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{id}/roles/{roleId}")]
        public async Task<ActionResult> AddToRoleAsync(Guid id, Guid roleId)
        {
            await _userServiceAsync.AddToRolesAsync(id, roleId);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AddToRolesAsync(Guid id, [FromBody] RoleIdsDTO roleIdsDTO)
        {
            await _userServiceAsync.AddToRolesAsync(id, roleIdsDTO.RoleIds);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _userServiceAsync.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<UserListDTO>>>> GetAllUsers([FromQuery] UserFilterRequest request)
        {
            var pagedUsersResult = await _userServiceAsync.GetAllAsync(request);
            return Ok(pagedUsersResult);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDetailDTO>>> GetByIdAsync(Guid id)
        {
            var user = await _userServiceAsync.GetByIdAsync(id);
            return Ok(user);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}/Modules/{moduleId}")]
        public async Task<ActionResult<ApiResponse<ModulePermissionDTO>>> GetModuleAsync(Guid id, int moduleId)
        {
            var permissions = await _userServiceAsync.GetModuleAsync(id, moduleId);
            return Ok(permissions);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}/Modules")]
        public async Task<ActionResult<List<ModulePermissionDTO>>> GetModulesAsync(Guid id)
        {
            var permissions = await _userServiceAsync.GetModulesAsync(id);
            return Ok(permissions);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}/roles")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetRolesAsync(Guid id)
        {
            var response = await _userServiceAsync.GetRolesAsync(id);
            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}/Modules/{moduleId}")]
        public async Task<ActionResult> RemoveModuleAsync(Guid id, int moduleId)
        {
            await _userServiceAsync.RemoveModuleAsync(id, moduleId);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}/Modules/{moduleId}/permissions")]
        public async Task<ActionResult> RemovePermissionsFromModuleAsync(Guid id, int moduleId, [FromBody] ModulePermissionIdsDTO modulePermissionIdsDTO)
        {
            await _userServiceAsync.RemovePermissionsFromModuleAsync(id, moduleId, modulePermissionIdsDTO.PermissionIds);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}/roles/{roleId}")]
        public async Task<ActionResult> RemoveRoleAsync(Guid id, Guid roleId)
        {
            await _userServiceAsync.RemoveFromRolesAsync(id, roleId);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}/roles")]
        public async Task<ActionResult> RemoveRolesAsync(Guid id, [FromBody] RoleIdsDTO roleIdsDTO)
        {
            await _userServiceAsync.RemoveFromRolesAsync(id, roleIdsDTO.RoleIds);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            await _userServiceAsync.UpdateAsync(id, userUpdateDTO);
            return NoContent();
        }

        #endregion Actions
    }
}