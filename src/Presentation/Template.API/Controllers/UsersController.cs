using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Dtos.Users;
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

        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AddToRolesAsync(Guid id, [FromBody] RoleIdsDto roleIdsDTO)
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
        public async Task<ActionResult<ApiResponse<PagedCollection<UserListDto>>>> GetAllUsers([FromQuery] UserQueryParameters request)
        {
            var pagedUsersResult = await _userServiceAsync.GetProjectedPaginatedAsync<UserListDto, UserSpecification>(request);
            return Ok(pagedUsersResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetByIdAsync(Guid id)
        {
            var user = await _userServiceAsync.GetUserDetailAsync(id);
            return Ok(user);
        }

        [HttpGet("{id}/roles")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetRolesAsync(Guid id)
        {
            var response = await _userServiceAsync.GetRolesAsync(id);
            return Ok(response);
        }


        [HttpDelete("{id}/roles")]
        public async Task<ActionResult> RemoveRolesAsync(Guid id, [FromBody] RoleIdsDto roleIdsDTO)
        {
            await _userServiceAsync.RemoveFromRolesAsync(id, roleIdsDTO.RoleIds);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UserUpdateDto userUpdateDTO)
        {
            await _userServiceAsync.UpdateAsync(id, userUpdateDTO);
            return NoContent();
        }

        #endregion Actions
    }
}