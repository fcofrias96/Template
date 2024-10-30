using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Specification.V1;

namespace Template.Api.Controllers
{
    [Authorize(Roles = Roles.Admin)]
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

        [HttpPost("{id}/roles/{roleId}")]
        public async Task<ActionResult> AddToRoleAsync(Guid id, Guid roleId)
        {
            await _userServiceAsync.AddToRolesAsync(id, roleId);
            return NoContent();
        }

        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AddToRolesAsync(Guid id, [FromBody] RoleIdsDTO roleIdsDTO)
        {
            await _userServiceAsync.AddToRolesAsync(id, roleIdsDTO.RoleIds);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _userServiceAsync.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<UserListDTO>>>> GetAllUsers([FromQuery] UserFilterRequest request)
        {
            var pagedUsersResult = await _userServiceAsync.GetAllAsync(request);
            return Ok(pagedUsersResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDetailDTO>>> GetByIdAsync(Guid id)
        {
            var user = await _userServiceAsync.GetByIdAsync(id);
            return Ok(user);
        }

        //[HttpGet("{id}/Modules/{moduleId}")]
        //public async Task<ActionResult<ApiResponse<ResourcePermissionDto>>> GetModuleAsync(Guid id, int moduleId)
        //{
        //    var permissions = await _userServiceAsync.GetModuleAsync(id, moduleId);
        //    return Ok(permissions);
        //}

        //[HttpGet("{id}/Modules")]
        //public async Task<ActionResult<List<ResourcePermissionDto>>> GetModulesAsync(Guid id)
        //{
        //    var permissions = await _userServiceAsync.GetModulesAsync(id);
        //    return Ok(permissions);
        //}

        [HttpGet("{id}/roles")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetRolesAsync(Guid id)
        {
            var response = await _userServiceAsync.GetRolesAsync(id);
            return Ok(response);
        }

        [HttpDelete("{id}/roles/{roleId}")]
        public async Task<ActionResult> RemoveRoleAsync(Guid id, Guid roleId)
        {
            await _userServiceAsync.RemoveFromRolesAsync(id, roleId);
            return NoContent();
        }

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